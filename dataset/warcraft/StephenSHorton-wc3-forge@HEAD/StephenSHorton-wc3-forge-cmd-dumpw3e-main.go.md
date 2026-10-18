---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "cmd/dumpw3e/main.go"
ref: HEAD
---

package main

import (
	"fmt"
	"os"
	"sort"

	"github.com/StephenSHorton/wc3-forge/internal/formats/w3e"
)

func main() {
	data, err := os.ReadFile("C:/Users/4step/AppData/Local/Temp/enfo-hivewe-workspace/enfo-ffb-potion999/war3map.w3e")
	if err != nil {
		fmt.Println("err:", err)
		return
	}
	f, err := w3e.Parse(data)
	if err != nil {
		fmt.Println("parse err:", err)
		return
	}
	fmt.Printf("tileset=%c width=%d height=%d\n", f.Tileset, f.Width, f.Height)
	fmt.Println("GROUND palette:")
	for i, fc := range f.GroundTilesets {
		fmt.Printf("  [%d] %s\n", i, fc)
	}
	fmt.Println("CLIFF palette:")
	for i, fc := range f.CliffTilesets {
		fmt.Printf("  [%d] %s\n", i, fc)
	}

	// Manually mirror CliffTypes.slk groundtile lookup for THIS map:
	//   CIsn (Cliff Ice Snow)      -> Isnw (Ice_Snow)      -> ground idx 1
	//   CIrb (Cliff Ice RuneBricks) -> Irbk (Ice_RuneBricks) -> ground idx 0
	c2g := []int{1, 0}
	fmt.Println("c2g map (cliff palette idx -> ground palette idx):")
	for i, v := range c2g {
		fmt.Printf("  [%d] %s -> %d\n", i, f.CliffTilesets[i], v)
	}

	W := int(f.Width)
	H := int(f.Height)

	cellCliff := make([]bool, W*H)
	for j := 0; j < H-1; j++ {
		for i := 0; i < W-1; i++ {
			bl := j*W + i
			lh := f.Tiles[bl].LayerHeight
			if f.Tiles[bl+1].LayerHeight != lh ||
				f.Tiles[bl+W].LayerHeight != lh ||
				f.Tiles[bl+W+1].LayerHeight != lh {
				cellCliff[bl] = true
			}
		}
	}

	// realTexHiveWE — HiveWE rule: remap if (a_romp || (a_cliff && !corner_ramp))
	realTexHiveWE := func(i, j int) uint32 {
		idx := j*W + i
		raw := uint32(f.Tiles[idx].GroundTexIdx())
		anyCliff := false
		if i < W-1 && j < H-1 && cellCliff[idx] {
			anyCliff = true
		}
		if i > 0 && j < H-1 && cellCliff[idx-1] {
			anyCliff = true
		}
		if i < W-1 && j > 0 && cellCliff[idx-W] {
			anyCliff = true
		}
		if i > 0 && j > 0 && cellCliff[idx-W-1] {
			anyCliff = true
		}
		hasRamp := f.Tiles[idx].HasRamp()
		if anyCliff && !hasRamp {
			cti := f.Tiles[idx].CliffTexIdx()
			if cti == 15 {
				cti = 1
			}
			if int(cti) < len(c2g) && c2g[cti] >= 0 {
				return uint32(c2g[cti])
			}
		}
		return raw
	}

	// realTexOurs — our current widened-gate rule (no romp pre-pass simulated here either)
	realTexOurs := func(i, j int) uint32 {
		idx := j*W + i
		raw := uint32(f.Tiles[idx].GroundTexIdx())
		anyCliff := false
		if i < W-1 && j < H-1 && cellCliff[idx] {
			anyCliff = true
		} else if i > 0 && j < H-1 && cellCliff[idx-1] {
			anyCliff = true
		} else if i < W-1 && j > 0 && cellCliff[idx-W] {
			anyCliff = true
		} else if i > 0 && j > 0 && cellCliff[idx-W-1] {
			anyCliff = true
		}
		hasRamp := f.Tiles[idx].HasRamp()
		// our widened gate: enter if anyRomp OR anyCliff (we approximate anyRomp=0)
		if !anyCliff {
			return raw
		}
		// narrowed exemption: skip remap if this corner is ramp AND no cliff adjacency
		// but we already gated on anyCliff being true, so this branch is unreachable
		_ = hasRamp
		// our code DOES NOT have the `!corner_ramp[idx]` HiveWE condition;
		// it remaps even ramp corners as long as anyCliff is true
		cti := f.Tiles[idx].CliffTexIdx()
		if cti == 15 {
			cti = 1
		}
		if int(cti) < len(c2g) && c2g[cti] >= 0 {
			return uint32(c2g[cti])
		}
		return raw
	}

	for label, fn := range map[string]func(int, int) uint32{"HiveWE": realTexHiveWE, "OURS": realTexOurs} {
		fmt.Println()
		fmt.Printf("=== %s: per-corner real_tile_texture for (10..14, 64..69) ===\n", label)
		for j := 64; j <= 69; j++ {
			for i := 10; i <= 14; i++ {
				rt := fn(i, j)
				fmt.Printf("  (%2d,%2d) raw=%d -> rt=%d\n", i, j, f.Tiles[j*W+i].GroundTexIdx(), rt)
			}
		}
		fmt.Printf("=== %s: per-CELL unique palette count + mask for cells (10..13, 64..68) ===\n", label)
		for j := 64; j <= 68; j++ {
			for i := 10; i <= 13; i++ {
				pBL := fn(i, j)
				pBR := fn(i+1, j)
				pTL := fn(i, j+1)
				pTR := fn(i+1, j+1)
				set := map[uint32]bool{pBL: true, pBR: true, pTL: true, pTR: true}
				uniq := make([]uint32, 0, 4)
				for k := range set {
					uniq = append(uniq, k)
				}
				sort.Slice(uniq, func(a, b int) bool { return uniq[a] < uniq[b] })
				masks := make([]uint32, len(uniq))
				for k, u := range uniq {
					var m uint32
					if pBR == u {
						m |= 0x1
					}
					if pBL == u {
						m |= 0x2
					}
					if pTR == u {
						m |= 0x4
					}
					if pTL == u {
						m |= 0x8
					}
					masks[k] = m
				}
				fmt.Printf("  cell(%2d,%2d) BL=%d BR=%d TL=%d TR=%d  uniq=%v masks=%v\n",
					i, j, pBL, pBR, pTL, pTR, uniq, masks)
			}
		}
	}
}
