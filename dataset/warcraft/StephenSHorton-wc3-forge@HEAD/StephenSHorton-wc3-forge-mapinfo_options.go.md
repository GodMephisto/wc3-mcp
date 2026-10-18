---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "mapinfo_options.go"
ref: HEAD
---

package main

import (
	"log"
	"os"
	"path"
	"path/filepath"
	"sort"
	"strings"
	"sync"

	"github.com/StephenSHorton/wc3-forge/internal/forge"
	"github.com/StephenSHorton/wc3-forge/internal/formats/slk"
)

// Option-list helpers for the Map Info Editor. Each `List<Foo>()` returns the
// rows the picker needs in `{key, label}` form — keys are the wire-format
// values written through ApplyInfoUpdates, labels are the resolved display
// strings (WESTRING-substituted, color-codes-stripped where applicable).
//
// All three sources are cached on first read since the underlying SLK / INI
// data is process-wide constant (it lives in CASC). Cache invalidation isn't
// wired because the only way to change these would be to swap the CASC mount,
// which we don't support at runtime.

// LabeledOption is the common shape used by every picker: a wire value (key)
// and a human-readable label. Used by Weather, Sound Environment, Custom Light
// Tileset, and Campaign Loading Screen pickers.
type LabeledOption struct {
	Key   string `json:"key"`
	Label string `json:"label"`
}

// --- Weather (TerrainArt/Weather.slk) ---------------------------------------
//
// HiveWE reads the per-row "name" + "effectid" columns from Weather.slk; the
// effectid is the 4-char FourCC packed into Info.WeatherID. Names like
// "WESTRING_WEATHER_..." are resolved through the stock string table.

var (
	weatherOnce  sync.Once
	weatherCache []LabeledOption
)

func loadWeathers() []LabeledOption {
	weatherOnce.Do(func() {
		data, ok, err := readBaseAsset("TerrainArt/Weather.slk")
		if err != nil || !ok {
			log.Printf("mapinfo: Weather.slk load skip: ok=%v err=%v", ok, err)
			return
		}
		m := slk.New()
		if err := m.Load(data); err != nil {
			log.Printf("mapinfo: Weather.slk parse: %v", err)
			return
		}
		wes := wesStrings()
		out := make([]LabeledOption, 0, len(m.Rows))
		for _, row := range m.Rows {
			eff := strings.TrimSpace(row.String("effectid"))
			if eff == "" {
				continue
			}
			nameRaw := strings.TrimSpace(row.String("name"))
			label := wes.Resolve(nameRaw)
			if label == "" {
				label = nameRaw
			}
			if label == "" {
				label = eff
			}
			out = append(out, LabeledOption{Key: eff, Label: label})
		}
		sort.Slice(out, func(i, j int) bool { return out[i].Label < out[j].Label })
		weatherCache = out
		log.Printf("mapinfo: loaded %d weather effects", len(out))
	})
	return weatherCache
}

// ListWeathers returns the per-row entries of TerrainArt/Weather.slk in
// {key=effectid (FourCC), label=resolved name} form. A synthetic "(None)"
// entry is prepended so the picker can express "no weather override".
func (a *App) ListWeathers() []LabeledOption {
	stock := loadWeathers()
	out := make([]LabeledOption, 0, len(stock)+1)
	out = append(out, LabeledOption{Key: "", Label: "None"})
	out = append(out, stock...)
	return out
}

// --- Sound Environments (UI/SoundInfo/EnvironmentSounds.slk) ----------------

var (
	soundEnvOnce  sync.Once
	soundEnvCache []LabeledOption
)

func loadSoundEnvironments() []LabeledOption {
	soundEnvOnce.Do(func() {
		data, ok, err := readBaseAsset("UI/SoundInfo/EnvironmentSounds.slk")
		if err != nil || !ok {
			log.Printf("mapinfo: EnvironmentSounds.slk load skip: ok=%v err=%v", ok, err)
			return
		}
		m := slk.New()
		if err := m.Load(data); err != nil {
			log.Printf("mapinfo: EnvironmentSounds.slk parse: %v", err)
			return
		}
		wes := wesStrings()
		out := make([]LabeledOption, 0, len(m.Rows))
		for _, row := range m.Rows {
			envType := strings.TrimSpace(row.String("environmenttype"))
			if envType == "" {
				continue
			}
			displayRaw := strings.TrimSpace(row.String("displaytext"))
			label := wes.Resolve(displayRaw)
			if label == "" {
				label = displayRaw
			}
			if label == "" {
				label = envType
			}
			out = append(out, LabeledOption{Key: envType, Label: label})
		}
		sort.Slice(out, func(i, j int) bool { return out[i].Label < out[j].Label })
		soundEnvCache = out
		log.Printf("mapinfo: loaded %d sound environments", len(out))
	})
	return soundEnvCache
}

// ListSoundEnvironments returns the rows of EnvironmentSounds.slk for the
// Custom Sound Environment picker. {key=environmenttype, label=displaytext}.
func (a *App) ListSoundEnvironments() []LabeledOption {
	stock := loadSoundEnvironments()
	out := make([]LabeledOption, 0, len(stock)+1)
	out = append(out, LabeledOption{Key: "", Label: "Default"})
	out = append(out, stock...)
	return out
}

// --- Tilesets (UI/WorldEditData.txt [TileSets]) -----------------------------
//
// Used for the Custom Light Tileset picker. The section format is:
//   <letter>=<DisplayName>,<comma-separated-extras>
// where <letter> is the single-char tileset code stored in
// Info.CustomLightTileset.

var (
	tilesetOnce  sync.Once
	tilesetCache []LabeledOption
)

func loadTilesetOptions() []LabeledOption {
	tilesetOnce.Do(func() {
		data, ok, err := readBaseAsset("UI/WorldEditData.txt")
		if err != nil || !ok {
			log.Printf("mapinfo: WorldEditData.txt load skip (tilesets): ok=%v err=%v", ok, err)
			return
		}
		if len(data) >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF {
			data = data[3:]
		}
		wes := wesStrings()
		section := ""
		var out []LabeledOption
		for _, raw := range strings.Split(string(data), "\n") {
			line := strings.TrimSpace(raw)
			if line == "" || strings.HasPrefix(line, "//") || strings.HasPrefix(line, ";") {
				continue
			}
			if strings.HasPrefix(line, "[") && strings.HasSuffix(line, "]") {
				section = line[1 : len(line)-1]
				continue
			}
			if !strings.EqualFold(section, "TileSets") {
				continue
			}
			eq := strings.IndexByte(line, '=')
			if eq < 0 {
				continue
			}
			key := strings.TrimSpace(line[:eq])
			if len(key) == 0 {
				continue
			}
			val := strings.TrimSpace(line[eq+1:])
			label := val
			if idx := strings.IndexByte(val, ','); idx >= 0 {
				label = strings.TrimSpace(val[:idx])
			}
			label = wes.Resolve(label)
			out = append(out, LabeledOption{Key: key[:1], Label: label})
		}
		sort.Slice(out, func(i, j int) bool { return out[i].Label < out[j].Label })
		tilesetCache = out
		log.Printf("mapinfo: loaded %d tileset options", len(out))
	})
	return tilesetCache
}

// ListTilesetOptions returns the [TileSets] rows for the Custom Light Tileset
// picker. {key=single-letter code, label=resolved tileset name}.
func (a *App) ListTilesetOptions() []LabeledOption {
	stock := loadTilesetOptions()
	out := make([]LabeledOption, 0, len(stock)+1)
	out = append(out, LabeledOption{Key: "", Label: "Default (use map tileset)"})
	out = append(out, stock...)
	return out
}

// --- Loading Screens --------------------------------------------------------
//
// Two sources:
//   - Campaign (stock) screens, from UI/WorldEditData.txt's [LoadingScreens]
//     section. The wire key is the 0-based row index, written into
//     LoadingScreen.Number; LoadingScreen.Model stays empty.
//   - Imported screens, scanned from the loaded map's filesystem (folder
//     sources only — MPQ archive enumeration is deferred). The wire key is
//     the relative .mdx path; LoadingScreen.Number stays at -1.

// LoadingScreenOption is the picker shape for a campaign loading screen.
// Index goes into Info.LoadingScreen.Number; Label is the resolved name.
type LoadingScreenOption struct {
	Index int    `json:"index"`
	Label string `json:"label"`
}

var (
	campaignLoadingOnce  sync.Once
	campaignLoadingCache []LoadingScreenOption
)

func loadCampaignLoadingScreens() []LoadingScreenOption {
	campaignLoadingOnce.Do(func() {
		data, ok, err := readBaseAsset("UI/WorldEditData.txt")
		if err != nil || !ok {
			log.Printf("mapinfo: WorldEditData.txt load skip (loading): ok=%v err=%v", ok, err)
			return
		}
		if len(data) >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF {
			data = data[3:]
		}
		wes := wesStrings()
		section := ""
		// Stable iteration order — we sort by the original NN= index after the
		// scan so the wire key (the campaign number) stays meaningful.
		type row struct {
			idx   int
			label string
		}
		var rows []row
		for _, raw := range strings.Split(string(data), "\n") {
			line := strings.TrimSpace(raw)
			if line == "" || strings.HasPrefix(line, "//") || strings.HasPrefix(line, ";") {
				continue
			}
			if strings.HasPrefix(line, "[") && strings.HasSuffix(line, "]") {
				section = line[1 : len(line)-1]
				continue
			}
			if !strings.EqualFold(section, "LoadingScreens") {
				continue
			}
			eq := strings.IndexByte(line, '=')
			if eq < 0 {
				continue
			}
			key := strings.TrimSpace(line[:eq])
			if strings.EqualFold(key, "NumScreens") {
				continue
			}
			val := strings.TrimSpace(line[eq+1:])
			// Format per row: NN=mdlpath,WESTRING_KEY[,extras...]. HiveWE
			// reads the second comma-separated field as the display name.
			parts := strings.SplitN(val, ",", 3)
			if len(parts) < 2 {
				continue
			}
			label := wes.Resolve(strings.TrimSpace(parts[1]))
			n := atoiSafe(key)
			rows = append(rows, row{idx: n, label: label})
		}
		sort.Slice(rows, func(i, j int) bool { return rows[i].idx < rows[j].idx })
		out := make([]LoadingScreenOption, 0, len(rows))
		for _, r := range rows {
			out = append(out, LoadingScreenOption{Index: r.idx, Label: r.label})
		}
		campaignLoadingCache = out
		log.Printf("mapinfo: loaded %d campaign loading screens", len(out))
	})
	return campaignLoadingCache
}

// ListCampaignLoadingScreens returns the stock [LoadingScreens] entries.
func (a *App) ListCampaignLoadingScreens() []LoadingScreenOption {
	return loadCampaignLoadingScreens()
}

// ListImportedLoadingScreens walks the currently-loaded map directory for
// .mdx files that can plausibly serve as imported loading screens. Returns
// paths relative to the map root, forward-slash normalized. Returns an empty
// slice (not error) when no map is loaded or the loaded map is an MPQ
// archive — the picker just shows "(none)" in that case.
//
// We intentionally don't filter by "looks like a loading screen model" — the
// editor convention is "user picked their own model" so any .mdx the map
// ships with is a candidate.
func (a *App) ListImportedLoadingScreens() []string {
	if !forge.Current.IsLoaded() {
		return []string{}
	}
	mapPath := forge.Current.Path()
	if mapPath == "" {
		return []string{}
	}
	info, err := os.Stat(mapPath)
	if err != nil || !info.IsDir() {
		return []string{}
	}
	var out []string
	_ = filepath.WalkDir(mapPath, func(p string, d os.DirEntry, err error) error {
		if err != nil {
			return nil
		}
		if d.IsDir() {
			return nil
		}
		if !strings.EqualFold(filepath.Ext(p), ".mdx") {
			return nil
		}
		rel, err := filepath.Rel(mapPath, p)
		if err != nil {
			return nil
		}
		out = append(out, path.Clean(strings.ReplaceAll(rel, `\`, "/")))
		return nil
	})
	sort.Strings(out)
	return out
}

// atoiSafe parses s as an int, returning 0 on error. Used for the campaign
// loading-screen NN= key; a non-numeric key is just skipped.
func atoiSafe(s string) int {
	n := 0
	neg := false
	if strings.HasPrefix(s, "-") {
		neg = true
		s = s[1:]
	}
	for _, ch := range s {
		if ch < '0' || ch > '9' {
			return 0
		}
		n = n*10 + int(ch-'0')
	}
	if neg {
		n = -n
	}
	return n
}
