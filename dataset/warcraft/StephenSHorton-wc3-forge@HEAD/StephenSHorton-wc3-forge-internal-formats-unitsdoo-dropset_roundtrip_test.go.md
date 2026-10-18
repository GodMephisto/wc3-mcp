---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/unitsdoo/dropset_roundtrip_test.go"
ref: HEAD
---

package unitsdoo

import (
	"bytes"
	"os"
	"path/filepath"
	"testing"
)

// Fixture 4: HiveWE's "test map" war3mapUnits.doo (214 bytes). version 8,
// SUBVERSION 9, 2 entities (a sloc + a "ratf"). The SUBVERSION-9-WITH-SKIN_ID
// counter-example: despite being subversion 9 (where skin_id is historically
// absent), this Reforged re-save carries a skin_id chunk on EVERY entity. Parse
// resolves that per-file (chooseSkinIDPresence tries absent-first, falls through
// to present), so both entities come back with skinIDPresent == true.
//
// This is the exact file that triggered the place-unit-then-save-then-reopen
// corruption: appending a freshly-constructed unit (parsed == false) used to
// fall back to the subversion rule (sub-9 => omit skin_id), producing a stream
// where the original two entities carry skin_id but the appended one does NOT.
// No uniform re-parse can consume that mix, so Parse misaligned and blew up on
// entity 0 with a bogus "drop_set[9].item[22].chance: unexpected EOF". The fix
// resolves skin_id presence FILE-WIDE in Encode (fileSkinIDPolicy) so every
// entity agrees.
var fixturePathHiveWESub9 = filepath.Join("testdata", "hivewe_test_map_sub9_skin.doo")

// TestRoundTrip_HiveWESub9 asserts byte-identical Parse -> Encode for the
// subversion-9-with-skin_id fixture. (a) from the task spec.
func TestRoundTrip_HiveWESub9(t *testing.T) {
	orig, err := os.ReadFile(fixturePathHiveWESub9)
	if err != nil {
		t.Fatalf("read fixture: %v", err)
	}
	f, err := Parse(orig)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	if f.SubVersion != 9 {
		t.Fatalf("SubVersion = %d, want 9", f.SubVersion)
	}
	// Sanity: the file carries skin_id on every entity despite subversion 9.
	for i := range f.Entities {
		if !f.Entities[i].skinIDPresent {
			t.Fatalf("entity[%d].skinIDPresent = false, want true (this sub-9 re-save carries skin_id)", i)
		}
	}
	out, err := Encode(f)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	if !bytes.Equal(orig, out) {
		t.Fatalf("Encode output != original (sizes orig=%d encoded=%d, first diff at offset %d)",
			len(orig), len(out), firstDiff(orig, out))
	}
}

// TestAppendUnit_ThenReparse is the format-level reproduction of the user's
// place-unit-then-save-then-reopen path. (b) from the task spec.
//
// It parses each real fixture, appends a freshly-synthesized unit that MIRRORS
// what Session.CreateUnit builds (see internal/forge/entities_mutate.go), then
// Encodes the whole file and re-Parses it. Before the fix, the subversion-9
// fixture failed here with "entity 0: drop_set[9].item[22].chance: unexpected
// EOF" because the appended unit omitted skin_id while the parsed entities
// carried it. After the fix, fileSkinIDPolicy makes Encode emit skin_id
// uniformly, so the stream re-parses cleanly and the new unit survives.
func TestAppendUnit_ThenReparse(t *testing.T) {
	fixtures := []string{
		fixturePathHiveWESub9, // the bug trigger (sub-9 + skin_id)
		fixturePathV16,        // sub-11 + skin_id
		fixturePathGreenCircle, // sub-11 WITHOUT skin_id
		fixturePathEnfo,
	}
	for _, path := range fixtures {
		t.Run(filepath.Base(path), func(t *testing.T) {
			orig, err := os.ReadFile(path)
			if err != nil {
				t.Fatalf("read fixture: %v", err)
			}
			f, err := Parse(orig)
			if err != nil {
				t.Fatalf("Parse: %v", err)
			}
			before := len(f.Entities)

			// Synthesize a new unit exactly as Session.CreateUnit does: SkinID
			// defaults to TypeID, RandomType 0 with a 4-byte payload, empty drop
			// sets (which must encode to a zero-set header), scaleRaw left zero
			// so Encode emits Scale*128, parsed == false.
			ne := Entity{
				TypeID:            "hpea",
				Variation:         0,
				Position:          [3]float32{128, 256, 0},
				Rotation:          0,
				Scale:             [3]float32{1, 1, 1},
				SkinID:            "hpea",
				Flags:             2,
				Player:            0,
				HitPointsPct:      -1,
				ManaPct:           -1,
				MapItemTable:      -1,
				GoldAmount:        0,
				TargetAcquisition: -1,
				HeroLevel:         1,
				RandomType:        0,
				RandomData:        make([]byte, 4),
				CustomColor:       -1,
				WaygateRegion:     -1,
				CreationNumber:    99999,
			}
			f.Entities = append(f.Entities, ne)

			out, err := Encode(f)
			if err != nil {
				t.Fatalf("Encode after append: %v", err)
			}
			f2, err := Parse(out)
			if err != nil {
				t.Fatalf("re-Parse after append failed (place-unit-then-save-then-reopen corruption): %v", err)
			}
			if got := len(f2.Entities); got != before+1 {
				t.Fatalf("after append+roundtrip entity count = %d, want %d", got, before+1)
			}
			// The appended unit must come back intact (it's the last entity).
			last := f2.Entities[len(f2.Entities)-1]
			if last.TypeID != "hpea" || last.CreationNumber != 99999 {
				t.Fatalf("appended unit roundtrip mismatch: got TypeID=%q cn=%d", last.TypeID, last.CreationNumber)
			}
			// Empty drop sets must round-trip as zero sets, no entries.
			if len(last.ItemDrops) != 0 || len(last.itemDropSetSizes) != 0 {
				t.Fatalf("appended unit drop sets not empty after roundtrip: drops=%d sizes=%v",
					len(last.ItemDrops), last.itemDropSetSizes)
			}
		})
	}
}

// TestMultipleDropSets_RoundTrip directly exercises the per-set boundary
// reconstruction in Encode for a unit carrying MULTIPLE drop sets (the on-disk
// shape the Entity model flattens). It builds a file by hand with a unit that
// has 3 drop sets of differing sizes, Encodes it, re-Parses, and asserts the
// per-set boundaries (itemDropSetSizes) and the flattened drops survive a full
// Encode -> Parse -> Encode cycle byte-identically.
func TestMultipleDropSets_RoundTrip(t *testing.T) {
	base := Entity{
		TypeID:            "ngol",
		Variation:         0,
		Position:          [3]float32{0, 0, 0},
		Rotation:          0,
		Scale:             [3]float32{1, 1, 1},
		SkinID:            "ngol",
		Flags:             2,
		Player:            12,
		HitPointsPct:      -1,
		ManaPct:           -1,
		MapItemTable:      -1,
		TargetAcquisition: -1,
		HeroLevel:         1,
		RandomType:        0,
		RandomData:        make([]byte, 4),
		CustomColor:       -1,
		WaygateRegion:     -1,
		CreationNumber:    1,
		// 3 sets: sizes 2, 1, 3. Flattened ItemDrops in set order.
		ItemDrops: []ItemDrop{
			{ItemID: "ratf", Chance: 100}, {ItemID: "rde1", Chance: 50}, // set 0 (2)
			{ItemID: "rde2", Chance: 75}, // set 1 (1)
			{ItemID: "rhth", Chance: 10}, {ItemID: "rhpe", Chance: 20}, {ItemID: "rlif", Chance: 30}, // set 2 (3)
		},
		itemDropSetSizes: []uint32{2, 1, 3},
		parsed:           true,
		skinIDPresent:    true,
	}
	f := &File{
		Format:     [4]byte{'W', '3', 'd', 'o'},
		Version:    8,
		SubVersion: 11,
		Entities:   []Entity{base},
	}

	enc1, err := Encode(f)
	if err != nil {
		t.Fatalf("Encode: %v", err)
	}
	f2, err := Parse(enc1)
	if err != nil {
		t.Fatalf("Parse: %v", err)
	}
	got := f2.Entities[0]
	if want := []uint32{2, 1, 3}; !equalU32(got.itemDropSetSizes, want) {
		t.Fatalf("itemDropSetSizes = %v, want %v", got.itemDropSetSizes, want)
	}
	if len(got.ItemDrops) != 6 {
		t.Fatalf("ItemDrops len = %d, want 6", len(got.ItemDrops))
	}
	if got.ItemDrops[3].ItemID != "rhth" || got.ItemDrops[3].Chance != 10 {
		t.Fatalf("ItemDrops[3] = %+v, want {rhth 10}", got.ItemDrops[3])
	}
	// Encode -> Parse -> Encode must be byte-stable.
	enc2, err := Encode(f2)
	if err != nil {
		t.Fatalf("re-Encode: %v", err)
	}
	if !bytes.Equal(enc1, enc2) {
		t.Fatalf("re-encode mismatch at offset %d", firstDiff(enc1, enc2))
	}
}

func equalU32(a, b []uint32) bool {
	if len(a) != len(b) {
		return false
	}
	for i := range a {
		if a[i] != b[i] {
			return false
		}
	}
	return true
}
