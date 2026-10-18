---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/objects_import.go"
ref: HEAD
---

package forge

// Cross-map object importer. Copies custom objects (abilities/units/buffs/items/
// upgrades/destructables/doodads) from a SOURCE map into the currently-loaded
// map, faithfully — including the dependency graph: an ability's referenced
// buffs, the units it summons, those units' abilities, and any imported icon/
// model files. The source map is read through a standalone fileSource and never
// loaded into the Session, so the active map is untouched until we mutate it.
//
// Three phases: (1) discover the full dependency graph read-only; (2) create
// every object, choosing a final id per object — its source FourCC if free, or a
// freshly-allocated one when onCollision=="remap" and the id is already taken;
// (3) copy each object's fields, rewriting object-reference values through the
// remap table so the copied graph stays internally consistent.

import (
	"errors"
	"fmt"
	"os"
	"path/filepath"
	"strings"

	"github.com/StephenSHorton/wc3-forge/internal/formats/mpq"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3objmod"
)

// ImportRequest names one object to import, by kind + FourCC.
type ImportRequest struct {
	Kind string `json:"kind"`
	ID   string `json:"id"`
}

// ImportObjectsResult reports the outcome.
type ImportObjectsResult struct {
	Imported      []string `json:"imported"`       // "kind:id" (or "kind:id->newid" when remapped)
	Skipped       []string `json:"skipped"`        // already present, onCollision=="skip"
	Missing       []string `json:"missing"`        // requested but not a custom in source
	ImportedFiles []string `json:"imported_files"` // imported asset paths copied across
	Failed        []string `json:"failed"`         // "kind:id: reason"
}

type objRef struct{ kind, id string }

func (r objRef) key() string { return r.kind + ":" + r.id }

// openSourceReader opens a read-only fileSource over a .w3x/.w3m/.mpq or an
// extracted folder. Mirrors Session.Open's source selection without swapping
// any Session state.
func openSourceReader(path string) (fileSource, func(), error) {
	abs, err := filepath.Abs(path)
	if err != nil {
		return nil, nil, fmt.Errorf("resolve path: %w", err)
	}
	fi, err := os.Stat(abs)
	if err != nil {
		return nil, nil, fmt.Errorf("stat %q: %w", abs, err)
	}
	if fi.IsDir() {
		return folderSource{root: abs}, func() {}, nil
	}
	archive, err := mpq.Open(abs)
	if err != nil {
		return nil, nil, fmt.Errorf("open MPQ %q: %w", abs, err)
	}
	src := newMPQSource(archive, abs)
	return src, func() { _ = src.close() }, nil
}

// sourceObjects holds a source map's parsed custom objects, indexed kind -> id.
type sourceObjects struct {
	customs map[string]map[string]*w3objmod.CustomObject
}

func loadSourceObjects(src fileSource) *sourceObjects {
	so := &sourceObjects{customs: map[string]map[string]*w3objmod.CustomObject{}}
	for kind, cfg := range kindConfigs {
		b, ok, err := src.read(cfg.ShadowFile)
		if err != nil || !ok || len(b) == 0 {
			continue
		}
		f, err := w3objmod.Parse(b, cfg.ShadowOpt, nil) // nil fields => raw FourCC keys
		if err != nil || f == nil {
			continue
		}
		idx := make(map[string]*w3objmod.CustomObject, len(f.Customs))
		for i := range f.Customs {
			idx[f.Customs[i].ID] = &f.Customs[i]
		}
		so.customs[kind] = idx
	}
	return so
}

func (so *sourceObjects) custom(kind, id string) *w3objmod.CustomObject {
	if m := so.customs[kind]; m != nil {
		return m[id]
	}
	return nil
}

// kindForFieldType maps a field metadata Type to the object kind its values
// reference (comma-separated FourCCs), or "" if it is not an object reference.
func kindForFieldType(t string) string {
	switch t {
	case "abilityList", "abilitySkinList":
		return "abilities"
	case "unitList":
		return "units"
	case "buffList":
		return "buffs"
	case "upgradeList":
		return "upgrades"
	case "destructableList":
		return "destructables"
	case "doodadList":
		return "doodads"
	case "itemList":
		return "items"
	}
	return ""
}

// refKindsForField returns the object kind(s) a field's value references, or nil.
func refKindsForField(fm *ObjectFieldMeta) []string {
	if fm == nil {
		return nil
	}
	if fm.Type == "techList" {
		return []string{"units", "upgrades"} // requirements: mixed unit/upgrade list
	}
	if k := kindForFieldType(fm.Type); k != "" {
		return []string{k}
	}
	return nil
}

// ImportObjectsFromMap copies the requested objects plus their custom-object and
// imported-file dependencies from srcPath into the loaded map. onCollision is
// "skip" (default — keep the source FourCC, skip if it's already present) or
// "remap" (allocate a fresh id for any object whose source FourCC is taken, and
// rewrite references to it). The whole copy is one undo group.
func (s *Session) ImportObjectsFromMap(srcPath string, reqs []ImportRequest, onCollision string) (*ImportObjectsResult, error) {
	if Current.Info() == nil {
		return nil, errors.New("no map loaded")
	}
	remap := onCollision == "remap"
	src, closeSrc, err := openSourceReader(srcPath)
	if err != nil {
		return nil, err
	}
	defer closeSrc()
	so := loadSourceObjects(src)
	res := &ImportObjectsResult{}

	// --- Phase 1: discover the dependency graph (read-only). ---
	order := make([]objRef, 0, len(reqs))
	inSet := map[string]bool{}
	fileSet := map[string]bool{}
	queue := make([]objRef, 0, len(reqs))
	for _, r := range reqs {
		queue = append(queue, objRef{r.Kind, r.ID})
	}
	for len(queue) > 0 {
		cur := queue[0]
		queue = queue[1:]
		if inSet[cur.key()] {
			continue
		}
		c := so.custom(cur.kind, cur.id)
		if c == nil {
			continue // stock ref or absent; only requested-but-missing is reported below
		}
		inSet[cur.key()] = true
		order = append(order, cur)
		cfg := kindConfigFor(cur.kind)
		if cfg == nil {
			continue
		}
		_, meta, _ := loadObjectBase(cfg)
		visit := func(fourCC, val string) {
			if meta == nil {
				return
			}
			fm := meta.ByID[fourCC]
			if fm == nil {
				return
			}
			if fm.Type == "icon" || fm.Type == "model" {
				for _, p := range filePathCandidates(val) {
					fileSet[p] = true
				}
				return
			}
			for _, k := range refKindsForField(fm) {
				for _, tok := range splitFourCCs(val) {
					if so.custom(k, tok) != nil {
						queue = append(queue, objRef{k, tok})
					}
				}
			}
		}
		for fourCC, val := range c.Overrides {
			visit(fourCC, val)
		}
		for _, lv := range c.Levels {
			visit(lv.FourCC, lv.Value)
		}
	}
	for _, r := range reqs {
		if so.custom(r.Kind, r.ID) == nil {
			res.Missing = append(res.Missing, r.Kind+":"+r.ID)
		}
	}

	s.BeginUndoGroup(fmt.Sprintf("Import %d objects from map", len(reqs)))
	defer func() { _ = s.EndUndoGroup() }()

	// --- Phase 2: create objects, choosing final ids (remap collisions). ---
	finalID := map[string]string{} // "kind:srcid" -> final id
	created := make([]objRef, 0, len(order))
	for _, ref := range order {
		cfg := kindConfigFor(ref.kind)
		c := so.custom(ref.kind, ref.id)
		if objectExistsInTarget(s, cfg, ref.id) {
			if !remap {
				res.Skipped = append(res.Skipped, ref.key())
				continue
			}
			newID, err := s.AddCustomObject(cfg, "", c.BaseID) // "" => allocate a fresh id
			if err != nil {
				res.Failed = append(res.Failed, ref.key()+": "+err.Error())
				continue
			}
			finalID[ref.key()] = newID
			created = append(created, ref)
			continue
		}
		if _, err := s.AddCustomObject(cfg, ref.id, c.BaseID); err != nil {
			res.Failed = append(res.Failed, ref.key()+": "+err.Error())
			continue
		}
		finalID[ref.key()] = ref.id
		created = append(created, ref)
	}

	// --- Phase 3: copy fields, rewriting references through finalID. ---
	for _, ref := range created {
		cfg := kindConfigFor(ref.kind)
		c := so.custom(ref.kind, ref.id)
		fid := finalID[ref.key()]
		_, meta, _ := loadObjectBase(cfg)
		for fourCC, val := range c.Overrides {
			_ = s.SetObjectField(cfg, fid, fourCC, rewriteRefs(meta, finalID, fourCC, val))
		}
		for _, lv := range c.Levels {
			_ = s.SetObjectFieldLevel(cfg, fid, lv.FourCC, lv.Level, rewriteRefs(meta, finalID, lv.FourCC, lv.Value))
		}
		if fid != ref.id {
			res.Imported = append(res.Imported, ref.key()+"->"+fid)
		} else {
			res.Imported = append(res.Imported, ref.key())
		}
	}

	// --- Copy referenced imported files (custom assets present in the source). ---
	for p := range fileSet {
		if b, ok, _ := src.read(p); ok && len(b) > 0 {
			if _, err := s.AddImportFile(p, b); err == nil {
				res.ImportedFiles = append(res.ImportedFiles, p)
			}
		}
	}
	return res, nil
}

// rewriteRefs rewrites object-reference tokens in a list-type field value through
// the remap table, so a copied object points at the (possibly remapped) ids of
// its dependencies. Non-reference fields pass through unchanged.
func rewriteRefs(meta *ObjectMetadata, finalID map[string]string, fourCC, val string) string {
	if meta == nil {
		return val
	}
	kinds := refKindsForField(meta.ByID[fourCC])
	if len(kinds) == 0 {
		return val
	}
	toks := strings.Split(val, ",")
	changed := false
	for i, t := range toks {
		tt := strings.TrimSpace(t)
		for _, k := range kinds {
			if nf, ok := finalID[k+":"+tt]; ok && nf != tt {
				toks[i] = nf
				changed = true
				break
			}
		}
	}
	if !changed {
		return val
	}
	return strings.Join(toks, ",")
}

func objectExistsInTarget(s *Session, cfg *KindConfig, id string) bool {
	if mods := cfg.GetMods(s); mods != nil {
		for i := range mods.Customs {
			if mods.Customs[i].ID == id {
				return true
			}
		}
	}
	if base, _, _ := loadObjectBase(cfg); base != nil && base.Rows != nil {
		if _, ok := base.Rows[id]; ok {
			return true
		}
	}
	return false
}

func splitFourCCs(v string) []string {
	var out []string
	for _, tok := range strings.Split(v, ",") {
		tok = strings.TrimSpace(tok)
		if len(tok) == 4 && tok != "_" && tok != "\x00\x00\x00\x00" {
			out = append(out, tok)
		}
	}
	return out
}

// filePathCandidates returns the path forms to probe in the source archive for
// an icon/model field value (slash variants + a .blp fallback for extensionless
// icon stems).
func filePathCandidates(v string) []string {
	v = strings.TrimSpace(v)
	if v == "" || v == "_" {
		return nil
	}
	seen := map[string]bool{}
	var out []string
	add := func(p string) {
		if p != "" && !seen[p] {
			seen[p] = true
			out = append(out, p)
		}
	}
	add(v)
	add(strings.ReplaceAll(v, "/", "\\"))
	add(strings.ReplaceAll(v, "\\", "/"))
	if !strings.Contains(filepath.Base(strings.ReplaceAll(v, "\\", "/")), ".") {
		add(v + ".blp")
		add(strings.ReplaceAll(v, "/", "\\") + ".blp")
	}
	return out
}
