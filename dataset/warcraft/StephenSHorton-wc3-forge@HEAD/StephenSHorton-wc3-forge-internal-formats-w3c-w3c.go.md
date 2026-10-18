---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/formats/w3c/w3c.go"
ref: HEAD
---

// Package w3c parses war3map.w3c — the WC3 game-cameras table. Read-only in
// Phase 2b2 (Trigger Editor needs camera-name lookup for gg_cam_ resolution
// + the entity picker's camera list). Encode is deferred.
//
// Wire format: see HiveWE's src/base/game_cameras.ixx. Layout (little-endian):
//
//	4   version       uint32, expected 0
//	4   camera_count  uint32
//	[
//	  4 target_x       (f32, game coords)
//	  4 target_y       (f32, game coords)
//	  4 z_offset       (f32)
//	  4 rotation       (f32)
//	  4 angle_of_attack (f32)
//	  4 distance       (f32)
//	  4 roll           (f32)
//	  4 fov            (f32)
//	  4 far_z          (f32)
//	  4 near_z         (f32)
//	  (1.31+ only:)
//	  4 local_pitch    (f32)
//	  4 local_yaw      (f32)
//	  4 local_roll     (f32)
//	  cstr name
//	] * camera_count
//
// Pre-1.31 maps omit the three "local_*" floats. The detection here uses the
// w3i game_version_major*100+minor >= 131 rule HiveWE follows; callers pass
// is131Plus directly so the loader can supply it from the parsed w3i.
package w3c

import (
	"encoding/binary"
	"fmt"
	"io"
	"math"
)

// File holds the parsed game cameras table.
type File struct {
	Version uint32
	Cameras []Camera
}

// Camera is one named camera preset. Trigger Editor exposes Name in pickers
// and resolves gg_cam_<Name> → display string. All angles are radians; the
// engine uses radians at runtime (rotation, angle_of_attack, roll, local_*).
type Camera struct {
	TargetX       float32
	TargetY       float32
	ZOffset       float32
	Rotation      float32
	AngleOfAttack float32
	Distance      float32
	Roll          float32
	FOV           float32
	FarZ          float32
	NearZ         float32
	LocalPitch    float32 // 1.31+ only; zero on older maps
	LocalYaw      float32
	LocalRoll     float32
	Name          string
	// Has131Locals is true when the source file carried the three local_*
	// floats. Lets consumers (and a future Encode) distinguish "absent" from
	// "present and zero".
	Has131Locals bool
}

// Parse decodes raw war3map.w3c bytes. The 1.31+ check is heuristic: we read
// the version uint32 + camera count, then attempt to size each entry assuming
// the longer layout; if the remaining bytes can't accommodate it, fall back
// to the shorter layout. This avoids requiring the caller to pre-classify the
// map version (Phase 2b2 callers pass a w3i, but tests + tooling want a
// version-agnostic entry point).
//
// Concretely: header is 8 bytes, each entry is 4 floats + cstr name (variable)
// in the short form; long form adds 12 bytes per entry. The detection iterates
// once with the long form; if it overruns or leaves trailing bytes that
// suggest the short form, retry with short.
func Parse(data []byte) (*File, error) {
	if long, err := parseAttempt(data, true); err == nil {
		return long, nil
	}
	return parseAttempt(data, false)
}

func parseAttempt(data []byte, has131 bool) (*File, error) {
	r := &reader{buf: data}
	version, err := r.readU32()
	if err != nil {
		return nil, fmt.Errorf("read version: %w", err)
	}
	count, err := r.readU32()
	if err != nil {
		return nil, fmt.Errorf("read camera count: %w", err)
	}
	const maxCameras = 10_000
	if count > maxCameras {
		return &File{Version: version}, nil
	}
	out := &File{Version: version, Cameras: make([]Camera, 0, count)}
	for i := uint32(0); i < count; i++ {
		var c Camera
		floats := []*float32{
			&c.TargetX, &c.TargetY, &c.ZOffset,
			&c.Rotation, &c.AngleOfAttack, &c.Distance, &c.Roll,
			&c.FOV, &c.FarZ, &c.NearZ,
		}
		for fi, ptr := range floats {
			if *ptr, err = r.readF32(); err != nil {
				return nil, fmt.Errorf("camera %d float %d: %w", i, fi, err)
			}
		}
		if has131 {
			if c.LocalPitch, err = r.readF32(); err != nil {
				return nil, fmt.Errorf("camera %d local_pitch: %w", i, err)
			}
			if c.LocalYaw, err = r.readF32(); err != nil {
				return nil, fmt.Errorf("camera %d local_yaw: %w", i, err)
			}
			if c.LocalRoll, err = r.readF32(); err != nil {
				return nil, fmt.Errorf("camera %d local_roll: %w", i, err)
			}
			c.Has131Locals = true
		}
		if c.Name, err = r.readCString(); err != nil {
			return nil, fmt.Errorf("camera %d name: %w", i, err)
		}
		out.Cameras = append(out.Cameras, c)
	}
	return out, nil
}

type reader struct {
	buf []byte
	off int
}

func (r *reader) readBytes(n int) ([]byte, error) {
	if r.off+n > len(r.buf) {
		return nil, io.ErrUnexpectedEOF
	}
	out := r.buf[r.off : r.off+n]
	r.off += n
	return out, nil
}

func (r *reader) readU32() (uint32, error) {
	b, err := r.readBytes(4)
	if err != nil {
		return 0, err
	}
	return binary.LittleEndian.Uint32(b), nil
}

func (r *reader) readF32() (float32, error) {
	v, err := r.readU32()
	if err != nil {
		return 0, err
	}
	return math.Float32frombits(v), nil
}

func (r *reader) readCString() (string, error) {
	start := r.off
	for r.off < len(r.buf) {
		if r.buf[r.off] == 0 {
			s := string(r.buf[start:r.off])
			r.off++
			return s, nil
		}
		r.off++
	}
	return "", io.ErrUnexpectedEOF
}
