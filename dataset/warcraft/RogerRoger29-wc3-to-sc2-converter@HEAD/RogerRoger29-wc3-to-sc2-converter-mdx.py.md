---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "mdx.py"
ref: HEAD
---

"""Complete WarCraft 3 MDX (v800) reader.

Parses the chunked MDLX binary into plain Python dicts:
geometry (verts/normals/UVs/faces), rigid skin (matrix groups -> bone object ids),
node hierarchy (bones/helpers/attachments/events/cameras/collision) with pivots,
animation tracks (KGTR/KGRT/KGSC ... translation/rotation/scale, with interpolation
type and tangents), materials/layers, textures, sequences, cameras.

Pure-Python, no Blender dependency. Run as CLI to dump JSON + a summary.

WC3 coordinate system: right-handed, Z up, 1 unit ~= 1 game-world unit. Angles/quats (x,y,z,w).
Animation track key 'time' is in MILLISECONDS (matches SEQS frame ranges).
"""
from __future__ import annotations
import struct, sys, json
from typing import Any, Callable, Dict, List, Optional, Tuple, Union

INTERP = {0: "none", 1: "linear", 2: "hermite", 3: "bezier"}
FILTER = {0: "None", 1: "Transparent", 2: "Blend", 3: "Additive", 4: "AddAlpha", 5: "Modulate", 6: "Modulate2x"}


class Reader:
    def __init__(self, data: bytes, pos: int = 0) -> None:
        self.d = data
        self.p = pos

    def u8(self) -> int:
        v = self.d[self.p]; self.p += 1; return v

    def u16(self) -> int:
        v = struct.unpack_from("<H", self.d, self.p)[0]; self.p += 2; return v

    def u32(self) -> int:
        v = struct.unpack_from("<I", self.d, self.p)[0]; self.p += 4; return v

    def i32(self) -> int:
        v = struct.unpack_from("<i", self.d, self.p)[0]; self.p += 4; return v

    def f32(self) -> float:
        v = struct.unpack_from("<f", self.d, self.p)[0]; self.p += 4; return round(v, 6)

    def vec(self, n: int) -> List[float]:
        return [self.f32() for _ in range(n)]

    def tag(self) -> str:
        t = self.d[self.p:self.p + 4]; self.p += 4; return t.decode("ascii", "replace")

    def cstr(self, n: int) -> str:
        b = self.d[self.p:self.p + n]; self.p += n; return b.split(b"\x00")[0].decode("ascii", "replace")

    def peek_tag(self) -> str:
        return self.d[self.p:self.p + 4].decode("ascii", "replace")


# --- animation track block: optional KG** sub-chunk inside a node -------------
def read_track(r: Reader, want_tags: Tuple[str, ...], value_reader: Callable[[Reader], Any]) -> Optional[Dict[str, Any]]:
    """If the next tag is one of want_tags, read a keyframe track. Returns dict or None.
    Leaves r.p unchanged if no matching tag."""
    if r.p + 4 > len(r.d):
        return None
    t = r.peek_tag()
    if t not in want_tags:
        return None
    r.tag()
    nkeys = r.u32()
    interp = r.u32()
    gseq = r.i32()
    keys = []
    for _ in range(nkeys):
        frame = r.i32()  # milliseconds
        val = value_reader(r)
        key = {"t": frame, "v": val}
        if interp >= 2:  # hermite/bezier carry inTan/outTan of same shape
            key["in"] = value_reader(r)
            key["out"] = value_reader(r)
        keys.append(key)
    return {"tag": t, "interp": INTERP.get(interp, interp), "globalSeq": gseq, "keys": keys}


def read_node(r: Reader) -> Dict[str, Any]:
    """Generic MDLX node header (shared by BONE/LITE/HELP/ATCH/PREM/PRE2/RIBB/EVTS/CAMS-attached/CLID-ish).
    Returns (node_dict, end_pos). Node has inclusive size as first u32."""
    start = r.p
    size = r.u32()
    name = r.cstr(80)
    object_id = r.u32()
    parent_id = r.u32()
    flags = r.u32()
    node = {"name": name, "objectId": object_id,
            "parentId": (None if parent_id == 0xFFFFFFFF else parent_id),
            "flags": flags, "tracks": {}}
    end = start + size
    # optional transform tracks until node end
    while r.p < end:
        t = r.peek_tag()
        if t == "KGTR":
            node["tracks"]["translation"] = read_track(r, ("KGTR",), lambda rr: rr.vec(3))
        elif t == "KGRT":
            node["tracks"]["rotation"] = read_track(r, ("KGRT",), lambda rr: rr.vec(4))  # quat x,y,z,w
        elif t == "KGSC":
            node["tracks"]["scale"] = read_track(r, ("KGSC",), lambda rr: rr.vec(3))
        else:
            break
    r.p = end
    return node


PE2_FILTER = {0: "blend", 1: "additive", 2: "modulate", 3: "modulate2x", 4: "alphakey"}
PE2_HEADTAIL = {0: "head", 1: "tail", 2: "both"}

def parse_pre2(data: bytes, off: int, size: int) -> List[Dict[str, Any]]:
    """ParticleEmitter2 (WC3 v800).

    Layout per emitter:
      [u32 size][char[80] name][u32 oid][u32 pid][u32 flags]
      then optional KGTR/KGRT/KGSC animation tracks (variable-length)
      then fixed PRE2 fields (172 bytes: speeds, filter, colors, alphas, scaling, UV intervals, texId, etc.)
      then optional KP2V emission-visibility track at the tail.

    When no KG tracks are present the fixed fields start at rel-offset 96 (matching the original
    empirically-confirmed layout). When KG tracks ARE present the fixed-base shifts forward by the
    total byte-length of the tracks, so we compute it dynamically.
    """
    end = off + size
    p = off
    emitters = []

    while p < end:
        esize = struct.unpack_from("<I", data, p)[0]
        name = data[p + 4:p + 84].split(b"\x00")[0].decode("ascii", "replace")
        oid = struct.unpack_from("<I", data, p + 84)[0]
        pid = struct.unpack_from("<I", data, p + 88)[0]
        flags = struct.unpack_from("<I", data, p + 92)[0]

        # --- scan for optional KG** animation tracks (KGTR/KGRT/KGSC) ---
        r = Reader(data, p + 96)
        tracks = {}
        while r.p < p + esize:
            t = r.peek_tag()
            if t == "KGTR":
                tracks["translation"] = read_track(r, ("KGTR",), lambda rr: rr.vec(3))
            elif t == "KGRT":
                tracks["rotation"] = read_track(r, ("KGRT",), lambda rr: rr.vec(4))
            elif t == "KGSC":
                tracks["scale"] = read_track(r, ("KGSC",), lambda rr: rr.vec(3))
            else:
                break
        base = r.p - p  # dynamic offset where fixed PRE2 fields begin

        # Helper readers relative to the emitter start (p)
        def f(o):
            return round(struct.unpack_from("<f", data, p + base + o)[0], 6)

        def u(o):
            return struct.unpack_from("<I", data, p + base + o)[0]

        def i(o):
            return struct.unpack_from("<i", data, p + base + o)[0]

        filterMode = u(32)
        fm = PE2_FILTER.get(filterMode, "additive")
        em = {
            "name": name, "objectId": oid, "parentId": (None if pid == 0xFFFFFFFF else pid),
            "flags": flags, "tracks": tracks,
            "speed": f(0), "variation": f(4), "latitude": f(8), "gravity": f(12),
            "lifespan": f(16), "emissionRate": f(20), "width": f(24), "length": f(28),
            "filterMode": fm, "filterModeRaw": filterMode, "rows": u(36), "columns": u(40),
            "headOrTail": PE2_HEADTAIL.get(u(44), u(44)), "tailLength": f(48), "timeMid": f(52),
            "colors": [[f(56), f(60), f(64)], [f(68), f(72), f(76)], [f(80), f(84), f(88)]],
            "alphas": [data[p + base + 92], data[p + base + 93], data[p + base + 94]],
            "scaling": [f(99), f(103), f(107)],
            "textureId": i(159), "squirt": u(163), "priorityPlane": i(167), "replaceableId": u(171),
        }
        # KP2V emission-rate visibility track sits right after the 172-byte fixed block
        em["emission"] = None
        kp2v_off = p + base + 175
        if kp2v_off + 4 <= p + esize and data[kp2v_off:kp2v_off + 4] == b"KP2V":
            r2 = Reader(data, kp2v_off)
            em["emission"] = read_track(r2, ("KP2V",), lambda rr: rr.f32())
        emitters.append(em)
        p += esize
    return emitters


def parse_geoa(data: bytes, off: int, size: int) -> List[Dict[str, Any]]:
    """GEOA geoset animations: per-geoset static + animated alpha (KGAO) and color (KGAC)."""
    end = off + size
    r = Reader(data, off)
    out = []
    while r.p < end:
        start = r.p
        gsize = r.u32()
        alpha = r.f32()
        flags = r.u32()
        color = r.vec(3)
        geoset_id = r.u32()
        tracks = {}
        while r.p < start + gsize:
            t = r.peek_tag()
            if t == "KGAO":
                tracks["alpha"] = read_track(r, ("KGAO",), lambda rr: rr.f32())
            elif t == "KGAC":
                tracks["color"] = read_track(r, ("KGAC",), lambda rr: rr.vec(3))
            else:
                break
        out.append({"geosetId": geoset_id, "staticAlpha": alpha, "flags": flags, "color": color, "tracks": tracks})
        r.p = start + gsize
    return out


def parse(path: str) -> Dict[str, Any]:
    data = open(path, "rb").read()
    assert data[:4] == b"MDLX", "not MDLX"
    r = Reader(data, 4)
    out = {"chunks": [], "model": {}, "sequences": [], "globalSequences": [],
           "textures": [], "materials": [], "geosets": [], "nodes": {},
           "bones": [], "helpers": [], "attachments": [], "events": [],
           "cameras": [], "collision": [], "pivots": []}
    # top-level chunk table
    chunks = {}
    p = 4
    while p < len(data):
        tag = data[p:p + 4].decode("ascii", "replace")
        size = struct.unpack_from("<I", data, p + 4)[0]
        chunks[tag] = (p + 8, size)
        out["chunks"].append({"tag": tag, "off": p + 8, "size": size})
        p += 8 + size

    def chunk_reader(tag):
        off, size = chunks[tag]
        return Reader(data, off), off + size

    # MODL
    if "VERS" in chunks:
        out["model"]["version"] = struct.unpack_from("<I", data, chunks["VERS"][0])[0]
    if "MODL" in chunks:
        cr, _ = chunk_reader("MODL")
        out["model"]["name"] = cr.cstr(80)
        cr.cstr(260)  # animationFileName
        out["model"]["boundsRadius"] = cr.f32()
        out["model"]["min"] = cr.vec(3)
        out["model"]["max"] = cr.vec(3)
        out["model"]["blendTime"] = cr.u32()

    # SEQS
    if "SEQS" in chunks:
        off, size = chunks["SEQS"]
        for i in range(size // 132):
            b = off + i * 132
            name = data[b:b + 80].split(b"\x00")[0].decode("ascii", "replace")
            s, e = struct.unpack_from("<II", data, b + 80)
            flags = struct.unpack_from("<I", data, b + 88)[0]
            moveSpeed = struct.unpack_from("<f", data, b + 92)[0]
            rarity = struct.unpack_from("<f", data, b + 100)[0]
            out["sequences"].append({"name": name, "start": s, "end": e, "lenMs": e - s,
                                     "nonLooping": bool(flags & 1), "moveSpeed": round(moveSpeed, 4),
                                     "rarity": round(rarity, 4)})
    # GLBS (global sequences)
    if "GLBS" in chunks:
        off, size = chunks["GLBS"]
        out["globalSequences"] = list(struct.unpack_from("<%dI" % (size // 4), data, off))

    # TEXS
    if "TEXS" in chunks:
        off, size = chunks["TEXS"]
        for i in range(size // 268):
            b = off + i * 268
            repl = struct.unpack_from("<I", data, b)[0]
            pth = data[b + 4:b + 264].split(b"\x00")[0].decode("ascii", "replace")
            flags = struct.unpack_from("<I", data, b + 264)[0]
            out["textures"].append({"replaceableId": repl, "path": pth, "flags": flags})

    # MTLS
    if "MTLS" in chunks:
        cr, end = chunk_reader("MTLS")
        while cr.p < end:
            mstart = cr.p
            msize = cr.u32()
            prio = cr.u32()
            mflags = cr.u32()
            assert cr.tag() == "LAYS"
            nlay = cr.u32()
            layers = []
            for _ in range(nlay):
                lstart = cr.p
                lsize = cr.u32()
                fm = cr.u32(); sf = cr.u32(); tid = cr.u32(); taid = cr.i32(); cid = cr.u32(); alpha = cr.f32()
                lay = {"filter": FILTER.get(fm, fm), "shadingFlags": sf, "textureId": tid,
                       "texAnimId": (None if taid < 0 else taid), "coordId": cid, "staticAlpha": alpha,
                       "twoSided": bool(sf & 16), "unshaded": bool(sf & 1), "unfogged": bool(sf & 32),
                       "tracks": {}}
                lend = lstart + lsize
                while cr.p < lend:
                    t = cr.peek_tag()
                    if t == "KMTF":
                        lay["tracks"]["texId"] = read_track(cr, ("KMTF",), lambda rr: rr.i32())
                    elif t == "KMTA":
                        lay["tracks"]["alpha"] = read_track(cr, ("KMTA",), lambda rr: rr.f32())
                    else:
                        break
                cr.p = lend
                layers.append(lay)
            out["materials"].append({"priorityPlane": prio, "flags": mflags, "layers": layers})
            cr.p = mstart + msize

    # PIVT (pivot points - one vec3 per object id, in order)
    if "PIVT" in chunks:
        off, size = chunks["PIVT"]
        n = size // 12
        for i in range(n):
            out["pivots"].append([round(x, 6) for x in struct.unpack_from("<3f", data, off + i * 12)])

    # GEOS
    if "GEOS" in chunks:
        cr, end = chunk_reader("GEOS")
        gi = 0
        while cr.p < end:
            gstart = cr.p
            gsize = cr.u32()
            assert cr.tag() == "VRTX"; nv = cr.u32(); verts = [cr.vec(3) for _ in range(nv)]
            assert cr.tag() == "NRMS"; nn = cr.u32(); norms = [cr.vec(3) for _ in range(nn)]
            assert cr.tag() == "PTYP"; npt = cr.u32(); ptypes = [cr.u32() for _ in range(npt)]
            assert cr.tag() == "PCNT"; npc = cr.u32(); pcnts = [cr.u32() for _ in range(npc)]
            assert cr.tag() == "PVTX"; ni = cr.u32(); idx = [cr.u16() for _ in range(ni)]
            assert cr.tag() == "GNDX"; ng = cr.u32(); gndx = [cr.u8() for _ in range(ng)]
            assert cr.tag() == "MTGC"; nm = cr.u32(); mtgc = [cr.u32() for _ in range(nm)]
            assert cr.tag() == "MATS"; nmat = cr.u32(); mats = [cr.u32() for _ in range(nmat)]
            material_id = cr.u32()
            selGroup = cr.u32(); selFlags = cr.u32()
            bounds = cr.f32(); bmin = cr.vec(3); bmax = cr.vec(3)
            nextent = cr.u32()
            cr.p += nextent * 28  # per-sequence extents (radius+min+max)
            # UVAS / UVBS
            uvs = []
            if cr.peek_tag() == "UVAS":
                cr.tag(); ntex = cr.u32()
                for _ in range(ntex):
                    assert cr.tag() == "UVBS"; nuv = cr.u32()
                    uvs.append([cr.vec(2) for _ in range(nuv)])
            # build per-vertex bone (matrix) groups
            group_offsets = []
            acc = 0
            for c in mtgc:
                group_offsets.append(acc); acc += c
            vbones = []
            for v in range(nv):
                g = gndx[v]
                cnt = mtgc[g]
                o = group_offsets[g]
                vbones.append(mats[o:o + cnt])  # list of node objectIds influencing this vertex (rigid, equal weight)
            faces = [idx[i:i + 3] for i in range(0, len(idx), 3)]
            out["geosets"].append({
                "index": gi, "nverts": nv, "ntris": len(faces), "materialId": material_id,
                "verts": verts, "normals": norms, "uvs": (uvs[0] if uvs else []),
                "uvLayers": len(uvs), "faces": faces, "vertexBones": vbones,
                "matrixGroups": {"mtgc": mtgc, "mats": mats}, "bounds": {"r": bounds, "min": bmin, "max": bmax},
            })
            gi += 1
            cr.p = gstart + gsize

    # node chunks
    def read_node_chunk(tag, sink, extra_after=0):
        if tag not in chunks:
            return
        cr, end = chunk_reader(tag)
        while cr.p < end:
            node = read_node(cr)
            # extra fixed fields after the node for some chunk types
            if extra_after:
                node["_extra"] = [cr.u32() for _ in range(extra_after)]
            sink.append(node)
            out["nodes"][node["objectId"]] = node

    # BONE: node + geosetId + geosetAnimId
    read_node_chunk("BONE", out["bones"], extra_after=2)
    # HELP: node only
    read_node_chunk("HELP", out["helpers"])
    # EVTS: event objects with optional KEVT time tracks
    if "EVTS" in chunks:
        cr, end = chunk_reader("EVTS")
        while cr.p < end:
            node = read_node(cr)
            # KEVT track follows the node: a keyframe track of event fire times
            if cr.peek_tag() == "KEVT":
                node["tracks"]["eventTime"] = read_track(cr, ("KEVT",), lambda rr: rr.i32())
            out["events"].append(node)
            out["nodes"][node["objectId"]] = node

    # ATCH: attachment points — node + path[260] + attachmentId + optional KATV visibility
    if "ATCH" in chunks:
        cr, end = chunk_reader("ATCH")
        while cr.p < end:
            astart = cr.p
            node = read_node(cr)
            # After the generic node: 260-char path, u32 attachmentId, optional KATV visibility track
            node["path"] = cr.cstr(260)
            node["attachmentId"] = cr.u32()
            if cr.peek_tag() == "KATV":
                node["tracks"]["visibility"] = read_track(cr, ("KATV",), lambda rr: rr.f32())
            out["attachments"].append(node)
            out["nodes"][node["objectId"]] = node
            if cr.p <= astart:
                break

    # CAMS
    if "CAMS" in chunks:
        cr, end = chunk_reader("CAMS")
        while cr.p < end:
            cstart = cr.p
            csize = cr.u32()
            name = cr.cstr(80)
            pos = cr.vec(3); fov = cr.f32(); farClip = cr.f32(); nearClip = cr.f32(); target = cr.vec(3)
            out["cameras"].append({"name": name, "pos": pos, "fov": round(fov, 5),
                                   "far": farClip, "near": nearClip, "target": target})
            cr.p = cstart + csize

    # PRE2 particle emitters
    out["particles"] = []
    if "PRE2" in chunks:
        off, size = chunks["PRE2"]
        out["particles"] = parse_pre2(data, off, size)

    # GEOA geoset animations (alpha/color)
    out["geosetAnims"] = []
    if "GEOA" in chunks:
        off, size = chunks["GEOA"]
        out["geosetAnims"] = parse_geoa(data, off, size)

    return out


def summarize(m: Dict[str, Any]) -> str:
    L = []
    L.append("model=%s v=%s bounds_r=%.1f min=%s max=%s" % (
        m["model"].get("name"), m["model"].get("version"), m["model"].get("boundsRadius", 0),
        m["model"].get("min"), m["model"].get("max")))
    L.append("sequences=%d textures=%d materials=%d geosets=%d bones=%d helpers=%d attachments=%d cameras=%d pivots=%d" % (
        len(m["sequences"]), len(m["textures"]), len(m["materials"]), len(m["geosets"]),
        len(m["bones"]), len(m["helpers"]), len(m["attachments"]), len(m["cameras"]), len(m["pivots"])))
    for b in m["bones"]:
        tk = ",".join(sorted(b["tracks"].keys())) or "-"
        nkeys = {k: len(v["keys"]) for k, v in b["tracks"].items()}
        L.append("  bone id=%s '%s' parent=%s tracks=[%s] keys=%s" % (b["objectId"], b["name"], b["parentId"], tk, nkeys))
    for h in m["helpers"]:
        L.append("  helper id=%s '%s' parent=%s tracks=%s" % (h["objectId"], h["name"], h["parentId"], list(h["tracks"].keys())))
    for a in m["attachments"]:
        L.append("  attach id=%s '%s' parent=%s" % (a["objectId"], a["name"], a["parentId"]))
    for g in m["geosets"]:
        nb = sorted({tuple(vb) for vb in g["vertexBones"]})
        L.append("  geoset[%d] v=%d t=%d mat=%d uvLayers=%d distinctBoneGroups=%d" % (
            g["index"], g["nverts"], g["ntris"], g["materialId"], g["uvLayers"], len(nb)))
    for c in m["cameras"]:
        L.append("  camera '%s' pos=%s target=%s fov=%.3f" % (c["name"], c["pos"], c["target"], c["fov"]))
    return "\n".join(L)


if __name__ == "__main__":
    path = sys.argv[1]
    m = parse(path)
    out_json = sys.argv[2] if len(sys.argv) > 2 else None
    print(summarize(m))
    if out_json:
        json.dump(m, open(out_json, "w"), indent=1)
        print("\nwrote", out_json)
