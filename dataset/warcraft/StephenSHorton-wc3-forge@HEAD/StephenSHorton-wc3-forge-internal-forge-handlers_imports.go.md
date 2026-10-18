---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/handlers_imports.go"
ref: HEAD
---

package forge

import (
	"encoding/base64"
	"encoding/json"
	"fmt"
)

// handlers_imports.go holds the MCP bridge handlers for the general Import
// Manager — list/add/remove/rename of arbitrary files in the loaded map's
// archive, registered in war3map.imp. The reg() lines live in RegisterAll
// (handlers.go); the mutators live in imports_mutate.go. Kept in its own file
// so it mirrors the handlers_regions.go split.
//
// WIRE-METHOD CONTRACT (exact names + params):
//   imports.list    {} -> {imports: [ImportEntry, ...]}
//   imports.add     {path, data_base64} -> {path}
//   imports.remove  {path} -> {ok:true}
//   imports.rename  {old_path, new_path} -> {ok:true}
//
// `data_base64` carries the file bytes base64-encoded (the bridge wire is JSON,
// so binary must be encoded). An empty/absent data_base64 imports a zero-byte
// placeholder, which is valid. paths are in-archive (war3mapImported\foo.blp or
// any stock path for an override); forward or back slashes are accepted.

func handleImportsList(_ json.RawMessage) (any, error) {
	return map[string]any{"imports": Current.ListImports()}, nil
}

// importsAddParams matches the imports.add wire shape. Data is base64 because
// the JSON-RPC wire can't carry raw bytes; an empty string decodes to a
// zero-byte file (a valid import).
type importsAddParams struct {
	Path       string `json:"path"`
	DataBase64 string `json:"data_base64"`
}

type importsAddResponse struct {
	Path string `json:"path"`
}

func handleImportsAdd(params json.RawMessage) (any, error) {
	var p importsAddParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.Path == "" {
		return nil, fmt.Errorf("path is required")
	}
	var data []byte
	if p.DataBase64 != "" {
		decoded, err := base64.StdEncoding.DecodeString(p.DataBase64)
		if err != nil {
			return nil, fmt.Errorf("data_base64 is not valid base64: %w", err)
		}
		data = decoded
	}
	norm, err := Current.AddImportFile(p.Path, data)
	if err != nil {
		return nil, err
	}
	return importsAddResponse{Path: norm}, nil
}

type importsRemoveParams struct {
	Path string `json:"path"`
}

func handleImportsRemove(params json.RawMessage) (any, error) {
	var p importsRemoveParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.Path == "" {
		return nil, fmt.Errorf("path is required")
	}
	if err := Current.RemoveImport(p.Path); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}

type importsRenameParams struct {
	OldPath string `json:"old_path"`
	NewPath string `json:"new_path"`
}

func handleImportsRename(params json.RawMessage) (any, error) {
	var p importsRenameParams
	if err := json.Unmarshal(params, &p); err != nil {
		return nil, fmt.Errorf("invalid params: %w", err)
	}
	if p.OldPath == "" || p.NewPath == "" {
		return nil, fmt.Errorf("old_path and new_path are required")
	}
	if err := Current.RenameImport(p.OldPath, p.NewPath); err != nil {
		return nil, err
	}
	return map[string]any{"ok": true}, nil
}
