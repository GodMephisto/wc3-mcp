---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "START_HERE.txt"
ref: HEAD
---

WC3 to SC2 Model Converter - Read This First
=============================================

This turns a Warcraft 3 model (.mdx) into a StarCraft 2 model (.m3), with its
textures, skeleton, animations, team color, and particle effects. There's a
Naaru model included so you can test it before doing your own.


The Easy Way (Double-Click the EXE)
-----------------------------------

If you downloaded the .exe:
  1. Double-click wc3toSC2.exe
  2. Drag your .mdx file onto the window
  3. Click Convert All

The program handles everything: scale estimation, texture finding,
animation matching, and Blender setup. No config files needed.

For the One-Click Setup button in Settings, the program can download
Blender and the m3studio addon automatically. Just click it once.


The CLI Way (Command Line)
--------------------------

What you need (one-time setup)
------------------------------

1. Blender 4.4
   Get it from blender.org. Install it like any program.

2. The m3studio add-on (this is what actually writes the SC2 .m3 file)
   - Download it from: https://github.com/Solstice245/m3studio
     (green "Code" button -> Download ZIP)
   - Open Blender. Go to Edit > Preferences > Add-ons > Install.
   - Pick the ZIP you downloaded, then tick the checkbox to turn it on.

3. Python (the normal kind, from python.org - NOT the one inside Blender)
   After installing it, open a command prompt and run:
       pip install numpy pillow
   That's two helper libraries the converter needs.

You only do this setup once.


Test it on the Naaru first
--------------------------

1. Open a command prompt in this folder.
   (In the address bar of the folder, type  cmd  and press Enter.)

2. Run this:
       python convert.py examples/Naaru/Naaru.json

3. If Blender isn't found, open examples/Naaru/Naaru.json in Notepad and fix
   the "blender" line so it points at your blender.exe, then run it again.

When it finishes you'll have a Naaru.m3 and some .dds texture files in
examples/Naaru/out. That's a finished SC2 model. If that worked, you're good.


Doing your own model
---------------------

1. Put your .mdx and its .blp texture files in one folder (textures can be in a
   "Textures" subfolder, that's fine).

2. Copy config.example.json, rename it to something like mymodel.json, and open
   it in Notepad. Set:
       "mdx"          -> your model's file name
       "model_name"   -> what you want the SC2 model called
       "scale"        -> 0.04 to 0.06 is normal (WC3 models are huge by default)
       "blender"      -> path to your blender.exe

3. Run:
       python convert.py mymodel.json

4. Copy the .dds files it makes into your SC2 map/mod, and import the .m3.


When something goes wrong
-------------------------

"Blender not found"
    Set the "blender" path in your .json to your real blender.exe location.

"No module named numpy" (or pillow)
    You missed step 3 of setup. Run:  pip install numpy pillow

It says a texture is MISSING
    The converter couldn't find one of the model's .blp files. Put it next to
    the .mdx (or in the Textures folder) and run again. Standard WC3 textures
    (like star4.blp) have to be pulled out of your Warcraft 3 game files first.

A texture looks see-through where it should be solid (or the reverse)
    In your .json, find that texture under "textures" and add:
        "alpha_invert": true
    Run again.

The particle effects look too big, too small, or too sparse
    In your .json, change "particle_rate_scale" (more = more particles) and
    "particle_size_scale" (more = bigger). Or just tweak the emitter in the SC2
    editor after importing. WC3 and SC2 handle particles differently, so these
    won't match the original exactly - expect to nudge them.

The model is the wrong size in SC2
    Change "scale" in the .json, or just set the Actor's Scale in the editor.


What it can and can't do
------------------------

Works on classic Warcraft 3 models (the older .mdx format) and their .blp
textures. It does not read World of Warcraft files.

The mesh, skeleton, animations, textures and team color come over faithfully.
Particle effects are close but not exact - that's the one part you may want to
touch up by hand.

Full details are in README.md if you want them.
