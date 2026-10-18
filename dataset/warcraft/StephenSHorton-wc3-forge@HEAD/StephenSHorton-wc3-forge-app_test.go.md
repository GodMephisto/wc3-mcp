---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "app_test.go"
ref: HEAD
---

package main

import "testing"

// TestBuildWindowTitle covers all four quadrants of the title composer:
// {map | no map} × {label | no label} × dirty, plus the whitespace-only
// collapse for both segments. Pure function → no Wails wiring needed.
func TestBuildWindowTitle(t *testing.T) {
	const pidTag = " — PID 12345"
	cases := []struct {
		name    string
		mapName string
		label   string
		dirty   bool
		want    string
	}{
		{
			name: "default — no map, no label, clean",
			want: "wc3-forge — PID 12345",
		},
		{
			name:  "dirty default",
			dirty: true,
			want:  "* wc3-forge — PID 12345",
		},
		{
			name:    "map only",
			mapName: "My Map",
			want:    "My Map — PID 12345",
		},
		{
			name:  "label only",
			label: "testing cliff fix",
			want:  "wc3-forge — testing cliff fix — PID 12345",
		},
		{
			name:    "map + label",
			mapName: "My Map",
			label:   "testing cliff fix",
			want:    "My Map — testing cliff fix — PID 12345",
		},
		{
			name:    "map + label + dirty",
			mapName: "My Map",
			label:   "testing cliff fix",
			dirty:   true,
			want:    "* My Map — testing cliff fix — PID 12345",
		},
		{
			// Whitespace-only map name falls back to "wc3-forge" so we never
			// render "   — PID n" — important because some TRIGSTR_ resolutions
			// could leave a blank Info.Name for malformed maps.
			name:    "whitespace map name falls back",
			mapName: "   ",
			label:   "x",
			want:    "wc3-forge — x — PID 12345",
		},
		{
			// Whitespace-only label collapses to no-label rather than rendering
			// an empty " —  — " segment.
			name:    "whitespace label collapses",
			mapName: "My Map",
			label:   "   ",
			want:    "My Map — PID 12345",
		},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got := buildWindowTitle(tc.mapName, tc.label, tc.dirty, pidTag)
			if got != tc.want {
				t.Errorf("\n got  %q\n want %q", got, tc.want)
			}
		})
	}
}
