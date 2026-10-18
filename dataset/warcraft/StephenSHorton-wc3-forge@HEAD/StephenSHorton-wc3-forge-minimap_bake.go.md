---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "minimap_bake.go"
ref: HEAD
---

package main

import (
	"bytes"
	"fmt"
	"image"
	"image/color"
	"image/png"

	"github.com/StephenSHorton/wc3-forge/internal/blp"
	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
)

// Minimap baking. Warcraft III's minimap (war3mapMap.blp) is normally an
// author-baked image the World Editor regenerates from the terrain on save.
// wc3-forge's "New Map" flow writes a fresh terrain but no minimap, so the
// map opens with the panel showing "No minimap" until the user paints one in
// another tool. We close that gap by baking a minimap straight from the .w3e:
// one pixel per playable cell, colored by that cell's ground-tile texture
// color (the same CASC-sampled colors the 3D terrain renderer uses).
//
// Two consumers share this:
//   - new_map.go bakes a war3mapMap.blp INTO the new .w3x, so the map is
//     complete (the minimap also shows in-game and on the loading screen).
//   - App.GenerateMinimapBytes bakes from the currently-loaded session on the
//     fly, so the panel can render a terrain preview for ANY map that happens
//     to lack a baked minimap — not just ones created here.
//
// The image's pixel grid spans the full vertex extent (Width-1 × Height-1
// cells, centered on CenterOffset), which is exactly the coordinate convention
// Minimap.svelte uses to map image pixels → world coords for click-to-pan and
// the camera-frustum overlay. Row 0 of the .w3e grid is the BOTTOM row, while
// image row 0 is the TOP, so we flip vertically when writing pixels.

// bakeMinimapImage renders a per-cell terrain color image from a parsed .w3e.
// Returns an (Width-1)×(Height-1) RGBA image (minimum 1×1). Each pixel is the
// color of the cell's bottom-left corner ground tile, with the HiveWE cliff
// remap applied to cliff/ramp cells so mixed-tileset maps show the cliff's
// real ground color rather than a stale stored index.
func bakeMinimapImage(t *w3e.File) *image.RGBA {
	W := int(t.Width)
	H := int(t.Height)
	cw := W - 1
	ch := H - 1
	if cw < 1 {
		cw = 1
	}
	if ch < 1 {
		ch = 1
	}
	img := image.NewRGBA(image.Rect(0, 0, cw, ch))

	// Resolve one color per ground-palette entry up front. tileColor caches and
	// samples from CASC; if it comes back as the miss sentinel we substitute the
	// tileset's approximate color so a CASC-less environment still gets sensible
	// terrain hues instead of flat gray.
	paletteColors := make([][3]uint8, len(t.GroundTilesets))
	for i, fc := range t.GroundTilesets {
		c := tileColor(fc)
		if c == neutralTileFallback {
			c = tilesetFallbackColor(t.Tileset)
		}
		paletteColors[i] = c
	}
	colorForGround := func(idx uint8) [3]uint8 {
		if int(idx) < len(paletteColors) {
			return paletteColors[idx]
		}
		return tilesetFallbackColor(t.Tileset)
	}

	// Cliff→ground remap table (same as GetTerrain): for cliff/ramp cells the
	// stored ground index is often stale, so we substitute the cliff type's
	// groundtile. -1 entries mean "leave the stored index alone".
	c2g := CliffToGroundTexture(t.CliffTilesets, t.GroundTilesets)

	// Guard: a degenerate grid (no real cells) just fills with the tileset color.
	if W < 2 || H < 2 {
		fill := tilesetFallbackColor(t.Tileset)
		img.Set(0, 0, color.RGBA{R: fill[0], G: fill[1], B: fill[2], A: 255})
		return img
	}

	// A simple water tint: cells whose bottom-left corner has water render in the
	// tileset's deep-water color so lakes/oceans read as blue rather than ground.
	water := WaterColors(t.Tileset)
	waterRGB := [3]uint8{water.DeepMin[0], water.DeepMin[1], water.DeepMin[2]}

	for j := 0; j < ch; j++ {
		for i := 0; i < cw; i++ {
			bl := j*W + i
			tp := t.Tiles[bl]

			groundIdx := tp.GroundTexIdx()
			// Cliff cell? Any of the 4 corners at a different layer height.
			lh := tp.LayerHeight
			isCliff := t.Tiles[bl+1].LayerHeight != lh ||
				t.Tiles[bl+W].LayerHeight != lh ||
				t.Tiles[bl+W+1].LayerHeight != lh
			if isCliff {
				ci := int(tp.CliffTexIdx())
				if ci >= 0 && ci < len(c2g) && c2g[ci] >= 0 {
					groundIdx = uint8(c2g[ci])
				}
			}

			rgb := colorForGround(groundIdx)
			if tp.HasWater() {
				// Blend ground toward deep water (70% water) so shallow edges keep
				// a hint of the bottom while open water reads clearly blue.
				rgb = [3]uint8{
					uint8((uint16(rgb[0]) + 2*uint16(waterRGB[0])) / 3),
					uint8((uint16(rgb[1]) + 2*uint16(waterRGB[1])) / 3),
					uint8((uint16(rgb[2]) + 2*uint16(waterRGB[2])) / 3),
				}
			}

			// Flip vertically: grid row 0 (bottom) → image bottom row.
			iy := (ch - 1) - j
			img.Set(i, iy, color.RGBA{R: rgb[0], G: rgb[1], B: rgb[2], A: 255})
		}
	}
	return img
}

// BakeMinimapPNG renders the terrain minimap and encodes it as a PNG. This is
// what the editor's minimap PANEL consumes: PNG is lossless and browser-native,
// so the frontend renders it directly without going through mdx-m3-viewer's BLP
// decoder (which mishandles Go's 3-component JPEG-in-BLP output — the bytes
// decode fine in WC3 and gowarcraft3, but render as garbage in the viewer's JPG
// path). BLP is still used for the on-disk war3mapMap.blp (see BakeMinimapBLP).
func BakeMinimapPNG(t *w3e.File) ([]byte, error) {
	if t == nil {
		return nil, fmt.Errorf("minimap: nil terrain")
	}
	var buf bytes.Buffer
	if err := png.Encode(&buf, bakeMinimapImage(t)); err != nil {
		return nil, fmt.Errorf("minimap: encode png: %w", err)
	}
	return buf.Bytes(), nil
}

// BakeMinimapBLP renders the terrain minimap and encodes it as a BLP1/JPEG
// texture (the format war3mapMap.blp uses and that both WC3 and the frontend's
// mdx-m3-viewer decode). blp.Encode resizes to the nearest power of two and
// builds a full mip chain, so the small per-cell image becomes a clean minimap.
func BakeMinimapBLP(t *w3e.File) ([]byte, error) {
	if t == nil {
		return nil, fmt.Errorf("minimap: nil terrain")
	}
	img := bakeMinimapImage(t)
	out, err := blp.Encode(img)
	if err != nil {
		return nil, fmt.Errorf("minimap: encode blp: %w", err)
	}
	return out, nil
}
