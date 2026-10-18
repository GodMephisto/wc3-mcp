---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "build_m3.py"
ref: HEAD
---

"""Generalized Warcraft 3 MDX -> StarCraft 2 M3 builder (Blender 4.4 headless, via the m3studio addon).

Run INSIDE Blender (the orchestrator convert.py does this for you):
    blender --background --factory-startup --python build_m3.py -- <build_config.json>

The build config (written by convert.py) supplies the mdx path, output path, scale, the wc3-texId -> output
DDS filename map, animation-name overrides, and feature toggles. Everything else (materials, particles,
attachments, bounds, hit-test, camera) is DERIVED from the MDX itself, so this runs on any WC3 v800 model:

  - material blend mode   <- WC3 layer filterMode (Blend->ALPHAB, Additive/AddAlpha->ADD, Modulate->MOD, ...)
  - unshaded / two-sided  <- WC3 layer shadingFlags
  - team color glow       <- WC3 replaceable textures (replaceableId 1=team colour, 2=team glow) -> TEAMEMIS
  - skeleton / skinning   <- BONE/HELP + matrix groups (rigid)
  - 9-ish animations      <- SEQS + KGTR/KGRT/KGSC, baked per-frame (matrix_basis = rest^-1 . L_node . rest)
  - animated fades        <- KMTA (material alpha) x KGAO (geoset alpha), baked into the diffuse colour
  - particle emitters     <- PRE2 (+ KP2V emission gating), mapped to SC2 particle systems
  - bounds / hit-test     <- MODL bounds
  - portrait camera       <- CAMS

Coordinate systems: WC3 MDX and Blender are both Z-up right-handed (verts/pivots map 1:1). UV V is flipped.
m3studio hardcodes 30 fps for the M3 timeline.
"""
import bpy, gpu, addon_utils, sys, os, json, importlib
import mathutils
from mathutils import Vector, Matrix, Quaternion

# ---------------------------------------------------------------- args + config
argv = sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else []
CFG_PATH = argv[0]
CFG = json.load(open(CFG_PATH, "r", encoding="utf-8"))
SCRATCH = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, SCRATCH)
import mdx as mdxlib

MDX_PATH = CFG["mdx"]
OUT_M3 = CFG["out"]
SCALE = float(CFG.get("scale", 1.0))
NAME = CFG.get("model_name", "Model")
TEX_DIR = CFG.get("asset_texture_dir", "Assets\\Textures\\")
TEX_MAP = {int(k): v for k, v in CFG.get("textures", {}).items()}   # wc3 TEXS index -> dds filename
FEAT = CFG.get("features", {})
def feat(k, default=True):
    return bool(FEAT.get(k, default))
PARTICLE_RATE_SCALE = float(CFG.get("particle_rate_scale", 1.0))
PARTICLE_SIZE_SCALE = float(CFG.get("particle_size_scale", 1.0))
TEAM_COLOR = bool(CFG.get("team_color", True))
BS = "\\"

# Default WC3 sequence name -> SC2 animation token map (override/extend via config "anim_names").
DEFAULT_ANIM_NAMES = {
    "Stand": "Stand", "Stand - 1": "Stand", "Stand - 2": "Stand 02", "Stand - 3": "Stand 03",
    "Stand - 4": "Stand 04", "Stand Ready": "Stand", "Stand Hit": "Stand", "Stand Channel": "Stand",
    "Walk": "Walk", "Run": "Walk", "Walk Fast": "Walk",
    "Attack": "Attack", "Attack - 1": "Attack", "Attack - 2": "Attack 02", "Attack Slam": "Attack",
    "Spell": "Spell", "Spell Channel": "Spell Channel", "Spell Slam": "Spell",
    "Death": "Death", "Dissipate": "Death Disintegrate", "Decay": "Death Disintegrate",
    "Decay Flesh": "Death", "Decay Bone": "Death Disintegrate",
    "Birth": "Birth", "Portrait": "Portrait", "Portrait Talk": "Portrait",
}
ANIM_NAMES = dict(DEFAULT_ANIM_NAMES); ANIM_NAMES.update(CFG.get("anim_names", {}))

# WC3 filterMode -> SC2 m3studio blend_mode enum
FILTER_TO_BLEND = {"None": "OPAQUE", "Transparent": "ALPHAA", "Blend": "ALPHAB",
                   "Additive": "ADD", "AddAlpha": "ADD", "Modulate": "MOD", "Modulate2x": "MOD2"}

def clamp01(x):
    return min(max(x, 0.0), 1.0)

# ---------------------------------------------------------------- bootstrap m3studio
# --- Blender version guard ---
blender_ver = bpy.app.version
if blender_ver < (4, 4, 0):
    raise SystemExit(
        "Blender %d.%d.%d is too old.  This tool requires Blender >= 4.4.0 "
        "(the m3studio addon and M3 timeline hardcode 30 fps on 4.4)."
        % blender_ver)
print("Blender %d.%d.%d — version ok" % blender_ver)

# --- m3studio addon validation ---
gpu.shader.from_builtin = lambda *a, **k: None
bpy.types.SpaceView3D.draw_handler_add = staticmethod(lambda *a, **k: None)

m3studio_ok = addon_utils.enable("m3studio-main", default_set=False, persistent=False)
if m3studio_ok is None:
    raise SystemExit(
        "The m3studio addon was not found in Blender.\n"
        "  1. Download it from https://github.com/Solstice245/m3studio\n"
        "  2. In Blender: Edit > Preferences > Add-ons > Install… > pick the downloaded zip\n"
        "  3. Tick the checkbox to enable it, then quit Blender.\n"
        "  4. The addon folder must be named 'm3studio-main' (the GitHub zip default).")

try:
    shared = importlib.import_module("m3studio-main.shared")
    io_m3_export = importlib.import_module("m3studio-main.io_m3_export")
except ImportError as e:
    raise SystemExit(
        "Failed to import m3studio modules: %s\n"
        "Make sure the m3studio addon is installed and its folder is named 'm3studio-main'." % e)

if not hasattr(bpy.types.Object, "m3_materialrefs"):
    raise SystemExit(
        "m3studio addon loaded but its Blender properties are missing.\n"
        "This usually means the addon wasn't fully registered.  Try restarting Blender once "
        "with the addon enabled (open the GUI, confirm it shows in the sidebar, then quit).")
print("m3studio enabled — props validated")

for o in list(bpy.data.objects):
    bpy.data.objects.remove(o, do_unlink=True)

m = mdxlib.parse(MDX_PATH)
print("parsed %s: geosets=%d bones=%d helpers=%d materials=%d textures=%d sequences=%d particles=%d" % (
    NAME, len(m["geosets"]), len(m["bones"]), len(m["helpers"]), len(m["materials"]),
    len(m["textures"]), len(m["sequences"]), len(m["particles"])))

# ================================================================ 1) ARMATURE + BONES
skel_nodes = {}
for b in m["bones"]:
    skel_nodes[b["objectId"]] = b
for h in m["helpers"]:
    skel_nodes[h["objectId"]] = h
pivots = m["pivots"]

def pivot_of(oid):
    return (Vector(pivots[oid]) * SCALE) if oid < len(pivots) else Vector((0, 0, 0))

arm_data = bpy.data.armatures.new(NAME + "Arm")
arm = bpy.data.objects.new(NAME, arm_data)
bpy.context.scene.collection.objects.link(arm)
bpy.context.view_layer.objects.active = arm
arm.select_set(True)

bpy.ops.object.mode_set(mode="EDIT")
ebones = {}
oid_to_bonename = {}   # captured in EDIT mode; EditBone refs are invalid once we leave edit mode
BONE_LEN = 6.0 * SCALE
for oid, node in sorted(skel_nodes.items()):
    eb = arm_data.edit_bones.new(node["name"] or ("bone_%d" % oid))
    oid_to_bonename[oid] = eb.name   # Blender may dedup/sanitize; read the final name back now
    head = pivot_of(oid)
    eb.head = head
    eb.tail = head + Vector((0, 0, max(BONE_LEN, 0.05)))   # cosmetic; orientation comes from baked world matrices
    eb.roll = 0.0
    ebones[oid] = eb
for oid, node in skel_nodes.items():
    pid = node["parentId"]
    if pid is not None and pid in ebones:
        ebones[oid].parent = ebones[pid]
        ebones[oid].use_connect = False
bpy.ops.object.mode_set(mode="OBJECT")

rest_world = {}
for oid, node in skel_nodes.items():
    pb = arm.pose.bones.get(oid_to_bonename[oid])
    rest_world[oid] = pb.bone.matrix_local.copy()

def stamp_bone(pb):
    pb.bl_handle = shared.m3_handle_gen()
    lock = arm.m3_bone_id_lockers.add(); lock.bone = pb.bl_handle
    pb.m3_location_hex_id = shared.m3_anim_id_gen()
    pb.m3_rotation_hex_id = shared.m3_anim_id_gen()
    pb.m3_scale_hex_id = shared.m3_anim_id_gen()
    pb.m3_batching_hex_id = shared.m3_anim_id_gen()
    pb.m3_export_cull = False

for pb in arm.pose.bones:
    stamp_bone(pb)

root_oid = m["bones"][0]["objectId"] if m["bones"] else min(skel_nodes)
ROOT_BONE = oid_to_bonename[root_oid]

# ================================================================ 2) MESHES + skin weights
mesh_objs = []
for g in m["geosets"]:
    me = bpy.data.meshes.new("%s_geo%d" % (NAME, g["index"]))
    verts = [list(Vector(v) * SCALE) for v in g["verts"]]
    faces = [tuple(f) for f in g["faces"]]
    me.from_pydata(verts, [], faces)
    me.update()
    uvs = g["uvs"]
    if uvs:
        uvl = me.uv_layers.new(name="UVMap")
        for poly in me.polygons:
            for li in poly.loop_indices:
                u, v = uvs[me.loops[li].vertex_index]
                uvl.data[li].uv = (u, 1.0 - v)
    me.update()
    ob = bpy.data.objects.new("%s_geo%d" % (NAME, g["index"]), me)
    bpy.context.scene.collection.objects.link(ob)
    ob.parent = arm
    # Edge-Split so authored hard facets keep facet normals (the m3 exporter reads bmesh loop normals;
    # this matches m3studio's own import convention and avoids smudged smooth-shaded crystal edges).
    es = ob.modifiers.new("EdgeSplit", "EDGE_SPLIT")
    es.use_edge_angle = True; es.use_edge_sharp = False; es.split_angle = 0.61
    vgs = {}
    for vi, bone_ids in enumerate(g["vertexBones"]):
        bone_ids = [bid for bid in bone_ids if bid in oid_to_bonename] or [root_oid]
        w = 1.0 / len(bone_ids)
        for bid in bone_ids:
            name = oid_to_bonename[bid]
            vg = vgs.get(name) or ob.vertex_groups.get(name) or ob.vertex_groups.new(name=name)
            vgs[name] = vg
            vg.add([vi], w, "REPLACE")
    ob.modifiers.new("Armature", "ARMATURE").object = arm
    mesh_objs.append((ob, g))
print("built %d meshes" % len(mesh_objs))

# ================================================================ 3) MATERIALS (derived from WC3)
def texture_dds_for(tex_id):
    """Return the SC2 asset path for a WC3 TEXS index, or None if it's a replaceable/team texture."""
    if tex_id is None or tex_id < 0 or tex_id >= len(m["textures"]):
        return None
    t = m["textures"][tex_id]
    if t["replaceableId"] in (1, 2):     # team colour / team glow -> handled as team emissive, no file
        return None
    dds = TEX_MAP.get(tex_id)
    return (TEX_DIR + dds) if dds else None

def is_team_layer(layer):
    tid = layer["textureId"]
    return (0 <= tid < len(m["textures"])) and m["textures"][tid]["replaceableId"] in (1, 2)

def add_material_for_wc3(mi, mat_def):
    """Build one SC2 standard material from a WC3 material definition. Returns (mref, mat, diff_layer)."""
    layers = mat_def["layers"]
    name = "%s_mat%d" % (NAME, mi)
    mref = shared.m3_item_add(arm.m3_materialrefs, item_name=name)
    mat = shared.m3_item_add(arm.m3_materials_standard, item_name=name)
    mref.mat_type = "m3_materials_standard"; mref.mat_handle = mat.bl_handle

    # the diffuse layer = first file-backed layer (else first layer); its filter drives the material blend
    file_layers = [l for l in layers if not is_team_layer(l)]
    base = file_layers[0] if file_layers else layers[0]
    blend = FILTER_TO_BLEND.get(base["filter"], "ALPHAB")
    try: mat.blend_mode = blend
    except Exception as e: print("  blend warn mat[%d]: %s" % (mi, e))
    for flag, on in (("two_sided", base["twoSided"]), ("unshaded", base["unshaded"])):
        if on and hasattr(mat, flag):
            try: setattr(mat, flag, True)
            except Exception as e: print("  warn mat[%d] %s: %s" % (mi, flag, e))
    if blend in ("ADD", "ALPHAA"):  # additive/alpha-test = glow-like: don't cast/receive shadows, no hit
        for fl in ("no_shadows_cast", "no_shadows_receive", "no_hittest"):
            if hasattr(mat, fl):
                try: setattr(mat, fl, True)
                except Exception as e: print("  warn mat[%d] %s: %s" % (mi, fl, e))

    # diffuse layer
    diff_dds = texture_dds_for(base["textureId"])
    diff = shared.m3_item_add(arm.m3_materiallayers, item_name=name + "_diff")
    if diff_dds:
        diff.color_type = "BITMAP"; diff.color_bitmap = diff_dds
    else:
        diff.color_type = "COLOR"
        try: diff.color_value = (0.7, 0.7, 0.75, 1.0)
        except Exception: pass
    mat.layer_diff = diff.bl_handle

    # team-colour emissive: if any layer uses a replaceable team texture, add a TEAMEMIS emissive layer,
    # masked by the diffuse bitmap when available (so the team colour glows over the surface).
    if TEAM_COLOR and any(is_team_layer(l) for l in layers) and hasattr(mat, "layer_emis1"):
        emis = shared.m3_item_add(arm.m3_materiallayers, item_name=name + "_emis")
        if diff_dds:
            emis.color_type = "BITMAP"; emis.color_bitmap = diff_dds
        else:
            emis.color_type = "COLOR"
            try: emis.color_value = (1.0, 1.0, 1.0, 1.0)
            except Exception: pass
        mat.layer_emis1 = emis.bl_handle
        if hasattr(mat, "blend_mode_emis1"):
            try: mat.blend_mode_emis1 = "TEAMEMIS"
            except Exception: pass
    return mref, mat, diff

matref_for_wc3 = {}
mat_anim = {}   # wc3 matId -> diffuse layer handle (for KMTA/GEOA alpha baking)
for mi, mat_def in enumerate(m["materials"]):
    mref, mat, diff = add_material_for_wc3(mi, mat_def)
    matref_for_wc3[mi] = mref
    mat_anim[mi] = diff.bl_handle
    print("  mat[%d] blend=%s layers=%d -> %s" % (mi, mat.blend_mode, len(mat_def["layers"]), mref.name))

# fallback material if a geoset references a missing material index
if not matref_for_wc3:
    fb = shared.m3_item_add(arm.m3_materialrefs, "fallback")
    fbm = shared.m3_item_add(arm.m3_materials_standard, "fallback")
    fb.mat_type = "m3_materials_standard"; fb.mat_handle = fbm.bl_handle
    d = shared.m3_item_add(arm.m3_materiallayers, "fallback_diff"); d.color_type = "COLOR"
    fbm.layer_diff = d.bl_handle
    matref_for_wc3[0] = fb

for ob, g in mesh_objs:
    mref = matref_for_wc3.get(g["materialId"]) or list(matref_for_wc3.values())[0]
    batch = shared.m3_item_add(ob.m3_mesh_batches)
    batch.material.handle = mref.bl_handle
print("materials + batches assigned")

# ================================================================ 3.5) ANIMATIONS
FPS = 30.0  # default; may be overridden per sequence by _estimate_seq_fps

def _estimate_seq_fps(mdx_data):
    """Estimate optimal baking FPS from keyframe density across all tracks."""
    min_interval = float("inf")
    for node in mdx_data.get("nodes", {}).values():
        for track in node.get("tracks", {}).values():
            keys = track.get("keys", [])
            for a, b in zip(keys, keys[1:]):
                dt = b["t"] - a["t"]
                if 0 < dt < min_interval:
                    min_interval = dt
    if min_interval in (float("inf"), 0) or min_interval < 1:
        return 30.0
    fps = 1000.0 / min_interval
    return max(10.0, min(60.0, round(fps / 5.0) * 5.0))


def _reduce_keyframes(fcurves, tolerance=0.0005):
    """Remove redundant keyframes within linear interpolation tolerance."""
    removed = 0
    for fc in fcurves:
        pts = fc.keyframe_points
        i = 1
        while i < len(pts) - 1:
            t0, v0 = pts[i - 1].co
            t1, v1 = pts[i].co
            t2, v2 = pts[i + 1].co
            frac = (t1 - t0) / (t2 - t0) if t2 != t0 else 0.0
            if abs(v1 - (v0 + (v2 - v0) * frac)) < tolerance:
                pts.remove(pts[i])
                removed += 1
            else:
                i += 1
    return removed


def _interval(keys, t):
    """Binary-search the keyframe interval containing time `t`.  Returns (lo, hi, frac)."""
    if t <= keys[0]["t"]:
        return 0, 0, 0.0
    n = len(keys) - 1
    if t >= keys[n]["t"]:
        return n, n, 0.0
    lo, hi = 0, n
    while lo < hi - 1:
        mid = (lo + hi) // 2
        if keys[mid]["t"] <= t:
            lo = mid
        else:
            hi = mid
    span = keys[hi]["t"] - keys[lo]["t"]
    return lo, hi, ((t - keys[lo]["t"]) / span if span else 0.0)

def sample_scalar(track, t, default):
    if not track or not track["keys"]:
        return default
    keys = track["keys"]; i, j, s = _interval(keys, t)
    v0 = keys[i]["v"]
    if i == j:
        return v0
    v1 = keys[j]["v"]; interp = track["interp"]
    if interp == "none":
        return v0
    if interp in ("hermite", "bezier"):
        m0 = keys[i].get("out", v0); m1 = keys[j].get("in", v1)
        if interp == "hermite":
            return (2*s**3-3*s**2+1)*v0 + (s**3-2*s**2+s)*m0 + (-2*s**3+3*s**2)*v1 + (s**3-s**2)*m1
        u = 1 - s
        return u**3*v0 + 3*u**2*s*m0 + 3*u*s**2*m1 + s**3*v1
    return v0 + (v1 - v0) * s

def sample_vec(track, t, default):
    if not track or not track["keys"]:
        return Vector(default)
    keys = track["keys"]; i, j, s = _interval(keys, t)
    v0 = Vector(keys[i]["v"])
    if i == j:
        return v0
    v1 = Vector(keys[j]["v"]); interp = track["interp"]
    if interp in ("hermite", "bezier"):
        m0 = Vector(keys[i].get("out", keys[i]["v"])); m1 = Vector(keys[j].get("in", keys[j]["v"]))
        if interp == "hermite":
            return ((2*s**3-3*s**2+1)*v0 + (s**3-2*s**2+s)*m0 + (-2*s**3+3*s**2)*v1 + (s**3-s**2)*m1)
        u = 1 - s
        return (u**3)*v0 + 3*u**2*s*m0 + 3*u*s**2*m1 + s**3*v1
    if interp == "none":
        return v0
    return v0.lerp(v1, s)

def sample_quat(track, t):
    """Sample a quaternion track at time t.

    Uses Shoemake squad (spherical cubic interpolation) for hermite/bezier
    tangent tracks. Falls back to slerp for linear/none interpolation.
    """
    if not track or not track["keys"]:
        return Quaternion((1, 0, 0, 0))

    keys = track["keys"]; i, j, s = _interval(keys, t)

    def _to_quat(k):
        x, y, z, w = k["v"]; return Quaternion((w, x, y, z))

    q0 = _to_quat(keys[i])
    if i == j or track["interp"] == "none":
        return q0

    q1 = _to_quat(keys[j])
    if track["interp"] not in ("hermite", "bezier"):
        return q0.slerp(q1, s)

    # Cubic quaternion curve via squad
    out_tan = keys[i].get("out")
    in_tan = keys[j].get("in")
    if out_tan is None or in_tan is None:
        return q0.slerp(q1, s)

    # Tangents are quaternion values; compute squad control quaternions
    a = q0.copy()
    a.rotate(_to_quat({"v": list(out_tan)}))
    a.normalize()

    b = q1.copy()
    inv_in = [-in_tan[0], -in_tan[1], -in_tan[2], in_tan[3]]
    b.rotate(_to_quat({"v": inv_in}))
    b.normalize()

    # squad(q0,q1,a,b,t) = slerp(slerp(q0,q1,t), slerp(a,b,t), 2t(1-t))
    u = 2.0 * s * (1.0 - s)
    return q0.slerp(q1, s).slerp(a.slerp(b, s), u)

def node_local_matrix(node, t):
    trans = sample_vec(node["tracks"].get("translation"), t, (0, 0, 0)) * SCALE
    quat = sample_quat(node["tracks"].get("rotation"), t)
    scl = sample_vec(node["tracks"].get("scale"), t, (1, 1, 1))
    piv = pivot_of(node["objectId"])
    return (Matrix.Translation(trans) @ Matrix.Translation(piv) @ quat.to_matrix().to_4x4()
            @ Matrix.Diagonal((scl.x, scl.y, scl.z, 1.0)) @ Matrix.Translation(-piv))

def build_animations():
    node_by_name = {node["name"]: node for node in skel_nodes.values()}
    name_by_oid = oid_to_bonename
    node_by_bonename = {name_by_oid[oid]: node for oid, node in skel_nodes.items()}
    rest_inv = {oid: rest_world[oid].inverted() for oid in skel_nodes}
    arm.animation_data_create()
    if hasattr(arm, "m3_options"):
        try: arm.m3_options.update_anim_data = False
        except Exception: pass
    pbs = list(arm.pose.bones)
    seq_actions = {}
    used_names = {}
    seq_fps = _estimate_seq_fps(m)
    for seq in m["sequences"]:
        s_ms, e_ms = seq["start"], seq["end"]
        nframes = max(1, round((e_ms - s_ms) / 1000.0 * fps))
        act = bpy.data.actions.new("%s_%s" % (NAME, seq["name"]))
        data = {pb.name: ([[] for _ in range(3)], [[] for _ in range(4)], [[] for _ in range(3)]) for pb in pbs}
        prev_q = {}
        for jf in range(nframes + 1):
            t = s_ms + jf * (1000.0 / fps)
            for pb in pbs:
                node = node_by_bonename[pb.name]; oid = node["objectId"]
                Mb = rest_inv[oid] @ node_local_matrix(node, t) @ rest_world[oid]
                loc, q, scl = Mb.decompose()
                pq = prev_q.get(pb.name)
                if pq is not None and q.dot(pq) < 0:
                    q = -q
                prev_q[pb.name] = q
                dl, dr, ds = data[pb.name]
                for k in range(3): dl[k] += [float(jf), loc[k]]
                for k in range(4): dr[k] += [float(jf), q[k]]
                for k in range(3): ds[k] += [float(jf), scl[k]]
        for pb in pbs:
            dl, dr, ds = data[pb.name]
            for path, arrs, cnt in (("location", dl, 3), ("rotation_quaternion", dr, 4), ("scale", ds, 3)):
                for k in range(cnt):
                    fc = act.fcurves.new(pb.path_from_id(path), index=k, action_group=pb.name)
                    n = len(arrs[k]) // 2
                    fc.keyframe_points.add(n); fc.keyframe_points.foreach_set("co", arrs[k])
                    fc.keyframe_points.foreach_set("interpolation", [1] * n)
        nm = ANIM_NAMES.get(seq["name"], seq["name"])
        used_names[nm] = used_names.get(nm, 0) + 1
        if used_names[nm] > 1:  # avoid duplicate SC2 anim tokens (e.g. two unmapped "Stand"s)
            nm = "%s %02d" % (nm, used_names[nm])
        grp = shared.m3_item_add(arm.m3_animation_groups, nm)
        grp["frame_start"] = 0; grp["frame_end"] = nframes
        for attr, val in (("frequency", 100), ("movement_speed", float(seq.get("moveSpeed", 0.0))),
                          ("not_looping", bool(seq.get("nonLooping", False)))):
            try: setattr(grp, attr, val)
            except Exception as e: print("  warn anim '%s' attr '%s': %s" % (nm, attr, e))
        sub = shared.m3_item_add(grp.animations, "full"); sub.action = act
        seq_actions[seq["name"]] = (act, nframes)
        print("  anim '%s' -> '%s'  frames 0..%d" % (seq["name"], nm, nframes))
    return seq_actions

seq_actions = {}
if feat("animations"):
    seq_actions = build_animations()

# ================================================================ 4) BOUNDS
mn = [c * SCALE for c in m["model"].get("min", [-50, -50, -50])]
mx = [c * SCALE for c in m["model"].get("max", [50, 50, 50])]
if hasattr(arm, "m3_bounds"):
    try:
        arm.m3_bounds.left, arm.m3_bounds.back, arm.m3_bounds.bottom = mn[0], mn[1], mn[2]
        arm.m3_bounds.right, arm.m3_bounds.front, arm.m3_bounds.top = mx[0], mx[1], mx[2]
    except Exception as e:
        print("bounds warn", e)

# ================================================================ 3.7) ATTACHMENT POINTS
def add_attachment(name, bone_name):
    pb = arm.pose.bones.get(bone_name)
    if not pb:
        return
    ap = shared.m3_item_add(arm.m3_attachmentpoints, name)
    try: ap.name = name
    except Exception: ap["name"] = name
    try: ap.bone.handle = pb.bl_handle
    except Exception as e: print("  attach warn", e)
    print("  attachment '%s' -> %s" % (name, bone_name))

if feat("attachments"):
    cfg_att = CFG.get("attachments")
    if cfg_att:
        for a in cfg_att:
            add_attachment(a["name"], a.get("bone") or ROOT_BONE)
    else:
        add_attachment("Ref_Origin", ROOT_BONE)   # required SC2 hardpoint; on the root bone
        add_attachment("Ref_Center", ROOT_BONE)

# ================================================================ 3.8) PARTICLE EMITTERS (PRE2)
WC3_PE_BLEND = {"additive": "ADD", "blend": "ALPHAB", "modulate": "MOD", "modulate2x": "MOD2", "alphakey": "ALPHAA"}
emitter_bones = []
if feat("particles") and m["particles"]:
    bpy.ops.object.mode_set(mode="EDIT")
    root_eb = arm_data.edit_bones.get(ROOT_BONE)
    for ei, e in enumerate(m["particles"]):
        bn = "PE_%d" % ei
        eb = arm_data.edit_bones.new(bn)
        piv = pivot_of(e["objectId"]) if e["objectId"] < len(pivots) else Vector((0, 0, 0))
        eb.head = piv; eb.tail = piv + Vector((0, 0, max(BONE_LEN, 0.05))); eb.roll = 0.0
        # parent to the emitter's WC3 parent bone if it's in the skeleton, else the root
        pid = e.get("parentId")
        eb.parent = arm_data.edit_bones.get(oid_to_bonename.get(pid, ROOT_BONE)) or root_eb
        emitter_bones.append(bn)
    bpy.ops.object.mode_set(mode="OBJECT")
    for bn in emitter_bones:
        stamp_bone(arm.pose.bones[bn])

    # particle materials (one per distinct WC3 texture used by an emitter)
    pmat = {}
    for e in m["particles"]:
        tid = e["textureId"]
        if tid in pmat:
            continue
        dds = texture_dds_for(tid)
        nm = "%s_PE_tex%d" % (NAME, tid)
        mref = shared.m3_item_add(arm.m3_materialrefs, nm)
        pm = shared.m3_item_add(arm.m3_materials_standard, nm)
        mref.mat_type = "m3_materials_standard"; mref.mat_handle = pm.bl_handle
        try: pm.blend_mode = WC3_PE_BLEND.get(e["filterMode"], "ADD")
        except Exception: pass
        for fl in ("unshaded", "two_sided", "no_shadows_cast", "no_shadows_receive", "no_hittest"):
            if hasattr(pm, fl):
                try: setattr(pm, fl, True)
                except Exception: pass
        d = shared.m3_item_add(arm.m3_materiallayers, nm + "_diff")
        if dds:
            d.color_type = "BITMAP"; d.color_bitmap = dds
        else:
            d.color_type = "COLOR"
        pm.layer_diff = d.bl_handle
        pmat[tid] = mref

    particle_anim = []   # (ps_handle, emitter) for KP2V baking
    for ei, e in enumerate(m["particles"]):
        ps = shared.m3_item_add(arm.m3_particlesystems, "%s_PE_%d" % (NAME, ei))
        ps.bone.handle = arm.pose.bones[emitter_bones[ei]].bl_handle
        mref = pmat.get(e["textureId"])
        if mref:
            ps.material.handle = mref.bl_handle

        def setp(name, val):
            if hasattr(ps, name):
                try: setattr(ps, name, val)
                except Exception as ex: print("  ps", name, "warn", ex)

        setp("particle_type", "BILLBOARD"); setp("emit_type", "RADIAL"); setp("emit_shape", "POINT")
        setp("vertex_alpha", True); setp("sort_method", "NONE")
        setp("size_smoothing", "LINEAR"); setp("color_smoothing", "LINEAR")
        # lifetime / rate (WC3 PRE2 emissionRate is particles/sec; many models use low rates -> particle_rate_scale).
        # WC3 lifespans range wildly; clamp to a sane window so a stray huge/zero value can't break the look.
        life = min(max(float(e.get("lifespan", 1.0)), 0.3), 8.0)
        setp("lifespan", life)
        base_rate = max(float(e.get("emissionRate", 0.0)) * PARTICLE_RATE_SCALE, 0.0)
        setp("emit_rate", base_rate)
        # motion: WC3 speed/gravity are in world units -> scale to SC2
        setp("emit_speed", float(e.get("speed", 0.0)) * SCALE)
        setp("emit_speed2", float(e.get("speed", 0.0)) * SCALE)
        if hasattr(ps, "gravity"):
            setp("gravity", float(e.get("gravity", 0.0)) * SCALE)
        # particle SIZE = WC3 'scaling' (start->end), which is the particle square's world-unit size (NOT
        # width/length, which are the emission-area dims). Scale to SC2 and clamp to the model's size so a
        # bad value can't produce a giant sprite. Tune globally with config "particle_size_scale".
        model_dim = max(mx[0] - mn[0], mx[2] - mn[2], 0.5)
        hi = max(model_dim * 0.6, 0.1)
        sc = e.get("scaling", [1.0, 1.0, 1.0]) or [1.0, 1.0, 1.0]
        s0 = min(max(float(sc[0]) * SCALE * PARTICLE_SIZE_SCALE, 0.02), hi)
        s2 = min(max(float(sc[-1]) * SCALE * PARTICLE_SIZE_SCALE, 0.02), hi)
        setp("size", (s0, s0, 0.0)); setp("size2", (s2, s2, 0.0))
        if hasattr(ps, "size_anim_mid"):
            setp("size_anim_mid", clamp01(float(e.get("timeMid", 0.5))))
        # colour + alpha envelope from WC3's 3 segments
        cols = e.get("colors", [[1, 1, 1]] * 3); al = e.get("alphas", [255, 255, 255])
        def col(i):
            c = cols[min(i, len(cols) - 1)]; a = al[min(i, len(al) - 1)] / 255.0
            return (clamp01(c[0]), clamp01(c[1]), clamp01(c[2]), clamp01(a))
        setp("color_init", col(0)); setp("color_mid", col(1)); setp("color_end", col(2))
        particle_anim.append((ps.bl_handle, e))
        print("  particle[%d] tex=%d blend=%s rate=%.2f life=%.2f size=%.2f->%.2f" % (
            ei, e["textureId"], WC3_PE_BLEND.get(e["filterMode"], "ADD"), base_rate, life, s0, s2))
else:
    particle_anim = []

# ================================================================ 3.9) HIT-TEST + CAMERA
if feat("hittest"):
    pb = arm.pose.bones.get(ROOT_BONE)
    if pb and hasattr(arm, "m3_hittest_tight"):
        try:
            cx = (mn[0] + mx[0]) / 2; cy = (mn[1] + mx[1]) / 2; cz = (mn[2] + mx[2]) / 2
            r = max((mx[0] - mn[0]), (mx[1] - mn[1]), (mx[2] - mn[2])) / 2 or 1.0
            ht = arm.m3_hittest_tight
            ht.bone.handle = pb.bl_handle; ht.shape = "SPHERE"
            ht.size = (r, r, r); ht.location = (cx, cy, cz)
            print("  hit-test sphere r=%.2f" % r)
        except Exception as e:
            print("  hittest warn", e)

if feat("camera") and m.get("cameras"):
    cam = m["cameras"][0]
    if hasattr(arm, "m3_cameras"):
        try:
            bpy.ops.object.mode_set(mode="EDIT")
            cbn = "Camera_Portrait"
            if cbn not in arm_data.edit_bones:
                ceb = arm_data.edit_bones.new(cbn)
                cpos = Vector(cam["pos"]) * SCALE
                ceb.head = cpos; ceb.tail = cpos + Vector((0, 0, max(BONE_LEN, 0.05))); ceb.parent = arm_data.edit_bones.get(ROOT_BONE)
            bpy.ops.object.mode_set(mode="OBJECT")
            cpb = arm.pose.bones[cbn]; stamp_bone(cpb)
            mc = shared.m3_item_add(arm.m3_cameras, "Portrait")
            try: mc.name = "Portrait"
            except Exception: mc["name"] = "Portrait"
            mc.bone.handle = cpb.bl_handle
            try: mc.field_of_view = cam.get("fov", 0.785)
            except Exception: pass
            try:
                mc.near_clip = max(cam.get("near", 1.0) * SCALE, 0.05)
                mc.far_clip = max(cam.get("far", 200.0) * SCALE, 10.0)
            except Exception: pass
            print("  portrait camera created")
        except Exception as e:
            print("  camera warn", e)

# ================================================================ 3.95) BAKE KMTA x KGAO (fade), KP2V (emission)
def _by_handle(coll, h):
    for it in coll:
        if it.bl_handle == h:
            return it
    return None

def bake_property_animations():
    if not seq_actions:
        return
    geoa = {ga["geosetId"]: ga["tracks"].get("alpha") for ga in m.get("geosetAnims", [])}
    geoset_mat = {g["index"]: g["materialId"] for g in m["geosets"]}
    mat_geoset = {}
    for gi, mi in geoset_mat.items():
        mat_geoset.setdefault(mi, gi)
    seq_fps = _estimate_seq_fps(m)
    for seq in m["sequences"]:
        if seq["name"] not in seq_actions:
            continue
        act, nframes = seq_actions[seq["name"]]
        s_ms = seq["start"]; frames = list(range(nframes + 1))
        # material/geoset alpha fade (e.g. Death dissolve)
        for matid, diff_handle in mat_anim.items():
            diff = _by_handle(arm.m3_materiallayers, diff_handle)
            if not diff:
                continue
            try: kmta = m["materials"][matid]["layers"][0]["tracks"].get("alpha")
            except Exception: kmta = None
            ga = geoa.get(mat_geoset.get(matid))
            if not kmta and not ga:
                continue
            vals = [clamp01(sample_scalar(kmta, s_ms + jf * (1000.0 / fps), 1.0)
                            * sample_scalar(ga, s_ms + jf * (1000.0 / fps), 1.0)) for jf in frames]
            if min(vals) < 0.999:
                diff.color_value_header.interpolation = "LINEAR"
                path = diff.path_from_id("color_value")
                for idx in range(4):   # keyframe all 4 (m3studio bug if only some vector components keyed)
                    fc = act.fcurves.find(path, index=idx) or act.fcurves.new(path, index=idx)
                    for jf, v in zip(frames, vals):
                        kp = fc.keyframe_points.insert(jf, v if idx == 3 else 1.0); kp.interpolation = "LINEAR"
        # geoset color animation (KGAC) — bake RGB into diffuse color_value
        geoa_col = {ga["geosetId"]: ga["tracks"].get("color") for ga in m.get("geosetAnims", [])}
        for matid, diff_handle in mat_anim.items():
            gc = geoa_col.get(mat_geoset.get(matid))
            if not gc or not gc.get("keys"):
                continue
            diff = _by_handle(arm.m3_materiallayers, diff_handle)
            if not diff:
                continue
            r_vals = [clamp01(sample_vec(gc, s_ms + jf * (1000.0 / fps), (1, 1, 1))) for jf in frames]
            if any(abs(v[0] - 1.0) > 0.001 or abs(v[1] - 1.0) > 0.001 or abs(v[2] - 1.0) > 0.001 for v in r_vals):
                diff.color_value_header.interpolation = "LINEAR"
                path = diff.path_from_id("color_value")
                for idx in range(4):
                    fc = act.fcurves.find(path, index=idx) or act.fcurves.new(path, index=idx)
                    for jf, v in zip(frames, r_vals):
                        kp = fc.keyframe_points.insert(jf, v[idx] if idx < 3 else 1.0)
                        kp.interpolation = "LINEAR"
                print("  baked KGAC color for mat[%d]" % matid)
        # particle emission gating (KP2V)
        for ps_handle, e in particle_anim:
            kp2v = e.get("emission")
            if not kp2v:
                continue
            ps = _by_handle(arm.m3_particlesystems, ps_handle)
            if not ps or not hasattr(ps, "emit_rate"):
                continue
            base = max(float(e.get("emissionRate", 0.0)) * PARTICLE_RATE_SCALE, 0.0)
            vals = [base * clamp01(sample_scalar(kp2v, s_ms + jf * (1000.0 / fps), 1.0)) for jf in frames]
            if max(vals) - min(vals) > 1e-4:
                ps.emit_rate_header.interpolation = "LINEAR"
                path = ps.path_from_id("emit_rate")
                fc = act.fcurves.find(path, index=0) or act.fcurves.new(path, index=0)
                for jf, v in zip(frames, vals):
                    kp = fc.keyframe_points.insert(jf, v); kp.interpolation = "LINEAR"
    print("baked KMTA/KGAO fade + KP2V emission")

if feat("animations"):
    bake_property_animations()

# ================================================================ 5) EXPORT
class StubOp:   # stand-in for the m3studio export operator (attributes the exporter reads)
    cull_unused_bones = False
    use_only_max_bounds = False
    output_anims = True
    section_reuse_mode = "EXPLICIT"
    cull_material_layers = False
    def report(self, *a, **k):
        print("  m3export.report:", a)
stub = StubOp()

bpy.context.view_layer.objects.active = arm
try:
    io_m3_export.m3_export(arm, OUT_M3, bl_op=stub)
    print("EXPORT_OK ->", OUT_M3, "size:", os.path.getsize(OUT_M3) if os.path.exists(OUT_M3) else 0)
except Exception as e:
    import traceback; traceback.print_exc()
    print("EXPORT_FAILED:", repr(e))
print("BUILD_DONE")
