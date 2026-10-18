---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "preview.py"
ref: HEAD
---

"""Model preview renderer — uses Blender subprocess to render a single frame.

Produces a PNG preview of the model's "Stand" pose for display in the GUI.
"""
from __future__ import annotations
import os, subprocess, tempfile
from typing import Optional


BLENDER_PREVIEW_SCRIPT = """
import bpy, sys, json, os
from mathutils import Vector, Matrix, Quaternion

# Clear scene
for o in list(bpy.data.objects):
    bpy.data.objects.remove(o, do_unlink=True)

mdx_path = sys.argv[-1]
sys.path.insert(0, os.path.dirname(__file__))
import mdx as mdxlib
m = mdxlib.parse(mdx_path)

scale = 0.05  # default for preview

# Quick armature + mesh
bones_data = m.get("bones", [])
helpers_data = m.get("helpers", [])
skel = {n["objectId"]: n for n in bones_data + helpers_data}
pivots = m.get("pivots", [])

def pivot_of(oid):
    return Vector(pivots[oid]) * scale if oid < len(pivots) else Vector((0,0,0))

arm_data = bpy.data.armatures.new("PrevArm")
arm = bpy.data.objects.new("Preview", arm_data)
bpy.context.scene.collection.objects.link(arm)
bpy.context.view_layer.objects.active = arm

bpy.ops.object.mode_set(mode="EDIT")
ebones = {}
bone_names = {}
for oid, node in sorted(skel.items()):
    eb = arm_data.edit_bones.new(node.get("name", "b%d"%oid))
    bone_names[oid] = eb.name
    head = pivot_of(oid)
    eb.head = head
    eb.tail = head + Vector((0, 0, max(6.0 * scale, 0.05)))
eb_list = list(arm_data.edit_bones)
for oid, node in skel.items():
    pid = node.get("parentId")
    if pid is not None and pid in skel:
        bn = bone_names.get(pid)
        if bn and bn in arm_data.edit_bones and bone_names[oid] in arm_data.edit_bones:
            arm_data.edit_bones[bone_names[oid]].parent = arm_data.edit_bones[bn]
bpy.ops.object.mode_set(mode="OBJECT")

root_oid = bones_data[0]["objectId"] if bones_data else min(skel)
root_name = bone_names[root_oid]

for g in m.get("geosets", []):
    me = bpy.data.meshes.new("geo%d" % g["index"])
    verts = [list(Vector(v) * scale) for v in g["verts"]]
    faces = [tuple(f) for f in g["faces"]]
    me.from_pydata(verts, [], faces)
    me.update()
    ob = bpy.data.objects.new("geo%d" % g["index"], me)
    bpy.context.scene.collection.objects.link(ob)
    ob.parent = arm
    vgs = {}
    for vi, bids in enumerate(g.get("vertexBones", [])):
        bids = [b for b in bids if b in bone_names] or [root_oid]
        w = 1.0 / len(bids)
        for bid in bids:
            bn = bone_names[bid]
            vg = vgs.get(bn) or ob.vertex_groups.get(bn) or ob.vertex_groups.new(name=bn)
            vgs[bn] = vg
            vg.add([vi], w, "REPLACE")
    ob.modifiers.new("Armature", "ARMATURE").object = arm

# Frame the camera
cam_data = bpy.data.cameras.new("PrevCam")
cam = bpy.data.objects.new("PrevCam", cam_data)
bpy.context.scene.collection.objects.link(cam)
bounds = m.get("model", {}).get("boundsRadius", 50) * scale
cam.location = (bounds * 2.5, -bounds * 2.5, bounds * 1.5)
cam.rotation_euler = (1.1, 0, 0.78)

# Light
light = bpy.data.lights.new("PrevLight", "SUN")
light.energy = 2.0
light_obj = bpy.data.objects.new("PrevLight", light)
bpy.context.scene.collection.objects.link(light_obj)
light_obj.rotation_euler = (1.0, 0.2, 0.5)

# Render settings
bpy.context.scene.camera = cam
bpy.context.scene.render.engine = "BLENDER_EEVEE"
bpy.context.scene.render.resolution_x = 512
bpy.context.scene.render.resolution_y = 512
bpy.context.scene.render.film_transparent = True
bpy.context.scene.render.image_settings.file_format = "PNG"
out_path = os.path.join(os.path.dirname(mdx_path), "_preview.png")
bpy.context.scene.render.filepath = out_path
bpy.ops.render.render(write_still=True)
print("PREVIEW_OK:" + out_path)
"""


def render_preview(mdx_path: str, blender_path: str = "blender",
                   output_size: int = 512) -> Optional[str]:
    """Render a preview PNG of the model using Blender headless.

    Returns the path to the generated PNG, or None on failure.
    """
    tmp = tempfile.NamedTemporaryFile(suffix=".py", mode="w", delete=False)
    tmp.write(BLENDER_PREVIEW_SCRIPT)
    tmp.close()

    try:
        proc = subprocess.run(
            [blender_path, "--background", "--factory-startup",
             "--python", tmp.name, "--", mdx_path],
            capture_output=True, text=True, timeout=30)

        for line in proc.stdout.splitlines():
            if "PREVIEW_OK:" in line:
                png_path = line.split("PREVIEW_OK:", 1)[1].strip()
                if os.path.exists(png_path):
                    return png_path

        return None
    except (subprocess.TimeoutExpired, FileNotFoundError, Exception):
        return None
    finally:
        try:
            os.unlink(tmp.name)
        except OSError:
            pass
