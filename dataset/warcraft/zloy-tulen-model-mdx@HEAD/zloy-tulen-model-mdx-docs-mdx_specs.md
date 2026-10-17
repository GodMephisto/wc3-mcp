---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "docs/mdx_specs.md"
ref: HEAD
---


[hiveworkshop.com](https://www.hiveworkshop.com/threads/mdx-specifications.240487/)

# MDX Specifications

GhostWolf

This is mostly based on Magos\' MDX specifications, but is more
accurate. Many of the keyframe track types where found by BlinkBoy.

The notation of this specifications is as follows:

Chunk names will always be given in their ASCII representation. For example, every MDX starts with the bytes \'M\', \'D\', \'L\' and
\'X\', or the string \"MDLX\". The MDX format uses this concept extensively, as a way to give chunks
meaningfull names. In your code, you will probably want to define constants for this, for
example `MDLX_CHUNK = 0x4d444c58`, and read the tags as integers, but
this is up to you. The (X) notation means X is optional, and may or may not exist. Flag variables hold different values in their bits with special
meanings. The notation for them would be the hexadecimal number representing the
correct bit. If for example there is 0x4, it means that the third bit holds specific
information which you can get with bitwise operators. That is, (Flag & 0x4) == 0x4 will show you if the value in the third bit
is true or false. Type variables hold only one value, and will simply be written using
normal decimal numbers.

MDX is made out of many chunks with no predefined order, especially
since they are all optional.

This is the main file structure:

C++:

``` cpp
MDX {
  char[4] "MDLX"
  (VERS)
  (MODL)
  (SEQS)
  (GLBS)
  (TEXS)
  (SNDS)
  (MTLS)
  (TXAN)
  (GEOS)
  (GEOA)
  (BONE)
  (LITE)
  (HELP)
  (ATCH)
  (PIVT)
  (PREM)
  (PRE2)
  (RIBB)
  (EVTS)
  (CAMS)
  (CLID)
  // The following chunks are for version > 800
  (BPOS)
  (FAFX)
  (CORN)
}
```

Each chunk starts with a header that consists of the chunk tag and the
chunk size in bytes, not including the size of the header itself.

C++:

``` cpp
Header {
  char[4] tag
  uint32 size
}
```

To parse the file you would generally have a loop which keeps reading
headers. If you recognize the header\'s tag and know how to parse it, do
it, otherwise just skip the header\'s size bytes.

C++:

``` cpp
Reader reader = ... // Some sort of binary reader

if (reader.read(4) == "MDLX") {
  while (reader.remaining() > 0) {
    Header header = ... // read 8 bytes and create a header
    if (canHandleTag(header.tag)) {
      // handle it
    } else {
      // skip it
      reader.skip(header.size)
    }
  }
}
```

Most of the chunks hold arrays of objects. In many of the cases you can\'t know how many objects there are before
parsing them, in which case the size will be written as \[?\] below. For example, each sequence is known to be 132 bytes. Since you know the
size of the sequence chunk from the header, you therefore know that you
have size/132 sequences. On the other hand, materials can have variable sizes, so you must resize
your array as you parse the chunk. To allow you to parse chunks with variable sized objects, these chunks
give you another size for each object they hold. For example, every material object first starts with a uint32 size,
which is the size of the material itself including this size variable,
and so we will call it inclusiveSize. So whenever you have a chunk with variable sized objects, you do
something along these lines:

C++:

``` cpp
Something[] somethings
uint32 totalSize = 0

while (totalSize < size) {
  uint32 inclusiveSize = ... // read a uint32
  somethings.push(new Something(...)) // construct a new object and add it to the array
  totalSize += inclusiveSize
}
```

There are a couple of exceptions that have no inclusive size variable,
and so are a bit more troublesome to handle, more on those later on.

With no further ado, the main chunks:


C++:

``` cpp
VERS {
  // 800 for Warcraft 3: RoC and TFT
  // >800 for Warcraft 3: Reforged
  uint32 version
}

MODL {
  char[80] name
  char[260] animationFileName
  Extent extent
  uint32 blendTime
}

SEQS {
  Sequence[size / 132] sequences
}

GLBS {
  uint32[size / 4] globalSequences
}

TEXS {
  Texture[size / 268] textures
}

// Note that this is here for completeness' sake.
// These objects were only used at some point before Warcraft 3 released.
SNDS {
  SoundTrack[size / 272] soundTracks
}

PIVT {
  float[size / 12][3] points
}

MTLS {
  Material[?] materials
}

TXAN {
  TextureAnimation[?] animations
}

GEOS {
  Geoset[?] geosets
}

GEOA {
  GeosetAnimation[?] animations
}

BONE {
  Bone[?] bones
}

LITE {
  Light[?] lights
}

HELP {
  Helper[?] helpers
}

ATCH {
  Attachment[?] attachments
}

// Emitters that emit models.
PREM {
  ParticleEmitter[?] emitters
}

// Emitters that emit quads.
PRE2 {
  ParticleEmitter2[?] emitters
}

// Emitters that emit lines, which are all connected together to form a ribbon of quads.
RIBB {
  RibbonEmitter[?] emitters
}

// A group of emitters that emit models, quads, and sounds.
EVTS {
  EventObject[?] objects
}

CAMS {
  Camera[?] cameras
}

CLID {
  CollisionShape[?] shapes
}

BPOS {
  uint32 count
  float[count][12] bindPose
}

// Face animations using the FaceFX runtime.
FAFX {
  FaceEffect[size / 380] faceEffects
}

// Emitters that use the PopcornFX runtime.
CORN {
  CornEmitter[?] emitters
}
```

And now to the actual meat, the objects themselves.


C++:

``` cpp
Extent {
  float boundsRadius
  float[3] minimum
  float[3] maximum
}

Node {
  uint32 inclusiveSize
  char[80] name
  uint32 objectId
  uint32 parentId
  uint32 flags // 0x0: helper
               // 0x1: dont inherit translation
               // 0x2: dont inherit rotation
               // 0x4: dont inherit scaling
               // 0x8: billboarded
               // 0x10: billboarded lock x
               // 0x20: billboarded lock y
               // 0x40: billboarded lock z
               // 0x80: camera anchored
               // 0x100: bone
               // 0x200: light
               // 0x400 event object
               // 0x800: attachment
               // 0x1000 particle emitter
               // 0x2000: collision shape
               // 0x4000: ribbon emitter
               // 0x8000: if particle emitter: emitter uses mdl, if particle emitter 2: unshaded
               // 0x10000: if particle emitter: emitter uses tga, if particle emitter 2: sort primitives far z
               // 0x20000: line emitter
               // 0x40000: unfogged
               // 0x80000: model space
               // 0x100000: xy quad
  (KGTR)
  (KGRT)
  (KGSC)
}

Sequence {
  char[80] name
  uint32[2] interval
  float moveSpeed
  uint32 flags // 0: looping
               // 1: non looping
  float rarity
  uint32 syncPoint
  Extent extent
}

Texture {
  uint32 replaceableId
  char[260] fileName
  uint32 flags
}

SoundTrack {
  char[260] fileName
  float volume
  float pitch
  uint32 flags
}

Material {
  uint32 inclusiveSize
  uint32 priorityPlane
  uint32 flags

  if (version > 800) {
    char[80] shader
  }

  char[4] "LAYS"
  uint32 layersCount
  Layer[layersCount] layers
}

Layer {
  uint32 inclusiveSize
  uint32 filterMode // 0: none
                    // 1: transparent
                    // 2: blend
                    // 3: additive
                    // 4: add alpha
                    // 5: modulate
                    // 6: modulate 2x
  uint32 shadingFlags // 0x1: unshaded
                      // 0x2: sphere environment map
                      // 0x4: ?
                      // 0x8: ?
                      // 0x10: two sided
                      // 0x20: unfogged
                      // 0x40: no depth test
                      // 0x80: no depth set
  uint32 textureId
  uint32 textureAnimationId
  uint32 coordId
  float alpha

  if (version > 800) {
    float emissiveGain
    float[3] fresnelColor
    float fresnelOpacity
    float fresnelTeamColor
  }

  (KMTF)
  (KMTA)
  if (version > 800) {
    (KMTE)
  }
  if (version > 900) {
    (KFC3)
    (KFCA)
    (KFTC)
  }
}

TextureAnimation {
  uint32 inclusiveSize
  (KTAT)
  (KTAR)
  (KTAS)
}

Geoset {
  uint32 inclusiveSize
  char[4] "VRTX"
  uint32 vertexCount
  float[vertexCount * 3] vertexPositions
  char[4] "NRMS"
  uint32 normalCount
  float[normalCount * 3] vertexNormals
  char[4] "PTYP"
  uint32 faceTypeGroupsCount
  uint32[faceTypeGroupsCount] faceTypeGroups // 0: points
                                             // 1: lines
                                             // 2: line loop
                                             // 3: line strip
                                             // 4: triangles
                                             // 5: triangle strip
                                             // 6: triangle fan
                                             // 7: quads
                                             // 8: quad strip
                                             // 9: polygons
  char[4] "PCNT"
  uint32 faceGroupsCount
  uint32[faceGroupsCount] faceGroups
  char[4] "PVTX"
  uint32 facesCount
  uint16[facesCount] faces
  char[4] "GNDX"
  uint32 vertexGroupsCount
  uint8[vertexGroupsCount] vertexGroups
  char[4] "MTGC"
  uint32 matrixGroupsCount
  uint32[matrixGroupsCount] matrixGroups
  char[4] "MATS"
  uint32 matrixIndicesCount
  uint32[matrixIndicesCount] matrixIndices
  uint32 materialId
  uint32 selectionGroup
  uint32 selectionFlags

  if (version > 800) {
    uint32 lod
    char[80] lodName
  }

  Extent extent
  uint32 extentsCount
  Extent[extentsCount] sequenceExtents

  if (version > 800) {
    (Tangents)
    (Skin)
  }

  char[4] "UVAS"
  uint32 textureCoordinateSetsCount
  TextureCoordinateSet[textureCoordinateSetsCount] textureCoordinateSets
}

Tangents {
  char[4] "TANG"
  uint32 count
  float[count * 4] tangents
}

Skin {
  char[4] "SKIN"
  uint32 count
  uint8[count] skin
}

TextureCoordinateSet {
  char[4] "UVBS"
  uint32 count
  float[count * 2] texutreCoordinates
}

GeosetAnimation {
  uint32 inclusiveSize
  float alpha
  uint32 flags
  float[3] color
  uint32 geosetId
  (KGAO)
  (KGAC)
}

Bone {
  Node node
  uint32 geosetId
  uint32 geosetAnimationId
}

Light {
  uint32 inclusiveSize
  Node node
  uint32 type // 0: omni light
              // 1: directional light
              // 2: ambient light
  float attenuationStart
  float attenuationEnd
  float[3] color
  float intensity
  float[3] ambientColor
  float ambientIntensity
  (KLAS)
  (KLAE)
  (KLAC)
  (KLAI)
  (KLBI)
  (KLBC)
  (KLAV)
}

Helper {
  Node node
}

Attachment {
  uint32 inclusiveSize
  Node node
  char[260] path
  uint32 attachmentId
  (KATV)
}

ParticleEmitter {
  uint32 inclusiveSize
  Node node
  float emissionRate
  float gravity
  float longitude
  float latitude
  char[260] spawnModelFileName
  float lifespan
  float initialiVelocity
  (KPEE)
  (KPEG)
  (KPLN)
  (KPLT)
  (KPEL)
  (KPES)
  (KPEV)
}

ParticleEmitter2 {
  uint32 inclusiveSize
  Node node
  float speed
  float variation
  float latitude
  float gravity
  float lifespan
  float emissionRate
  float length
  float width
  uint32 filterMode // 0: blend
                    // 1: additive
                    // 2: modulate
                    // 3: modulate 2x
                    // 4: alpha key
  uint32 rows
  uint32 columns
  uint32 headOrTail // 0: head
                    // 1: tail
                    // 2: both
  float tailLength
  float time
  float[3][3] segmentColor
  uint8[3] segmentAlpha
  float[3] segmentScaling
  uint32[3] headInterval
  uint32[3] headDecayInterval
  uint32[3] tailInterval
  uint32[3] tailDecayInterval
  uint32 textureId
  uint32 squirt
  uint32 priorityPlane
  uint32 replaceableId
  (KP2S)
  (KP2R)
  (KP2L)
  (KP2G)
  (KP2E)
  (KP2N)
  (KP2W)
  (KP2V)
}

RibbonEmitter {
  uint32 inclusiveSize
  Node node
  float heightAbove
  float heightBelow
  float alpha
  float[3] color
  float lifespan
  uint32 textureSlot
  uint32 emissionRate
  uint32 rows
  uint32 columns
  uint32 materialId
  float gravity
  (KRHA)
  (KRHB)
  (KRAL)
  (KRCO)
  (KRTX)
  (KRVS)
}

EventObject {
  Node node
  char[4] "KEVT"
  uint32 tracksCount
  uint32 globalSequenceId
  uint32[tracksCount] tracks
}

Camera {
  uint32 inclusiveSize
  char[80] name
  float[3] position
  float filedOfView
  float farClippingPlane
  float nearClippingPlane
  float[3] targetPosition
  (KCTR)
  (KTTR)
  (KCRL)
}

CollisionShape {
  Node node
  uint32 type // 0: cube
              // 1: plane
              // 2: sphere
              // 3: cylinder
  float[?][3] vertices // type 0: 2
                       // type 1: 2
                       // type 2: 1
                       // type 3: 2
  if (type == 2 || type == 3) {
    float radius
  }
}

FaceEffect {
  char[80] target
  char[260] path
}

CornEmitter {
  uint32 inclusiveSize
  Node node
  float lifeSpan
  float emissionRate
  float speed
  float[4] color
  uint32 replaceableId
  char[260] path
  char[260] flags
  (KPPA)
  (KPPC)
  (KPPE)
  (KPPL)
  (KPPS)
  (KPPV)
}
```

As you can see, bones, event objects and collision shapes have no
inclusive size, so they need different handling than the rest. For bones, we know that it starts with a Node object, followed by two
uint32s, so we know that the size of each bone is the inclusive size of
the node + 8. Event objects also have a node, so we use its inclusive size, but we
also need to get the size of the KEVT chunk if it exists. For collision shapes, we need the inclusive size of the node, in
addition to the shape\'s data, which is 28 bytes for cubes, and 16 bytes
for spheres.

If you want a unified way to parse all the variable-sized chunks, then
you can define the inclusive size by yourself for the chunks without
it. That is, for bones, define the inclusive size to be the node\'s
inclusive size + 8, for event objects define it as the inclusive size of
the node plus the size of the KEVT chunk if it exists, and so on. Once everything has correct inclusive sizes, you can do something like
this:


C++:

``` cpp
Something[] somethings
uint32 totalSize = 0

while (totalSize < size) {
  Something something = new Something(...) // construct a new object
  totalSize += something.inclusiveSize
  somethings.push(something)
}
```

Almost all of the optional fields in Wacraft 3 models are keyframe
tracks, or in other words, things that can change over time while the
animation runs. A Node for example can have KGTR, KGRT, and KGSC tracks, but it doesn\'t
mean it has to have them. It can have neither of them, one of them, two
of them or all three. All tracks except for KEVT (event objects), follow the same structure,
but with different types for the fields. The structure looks like this:


C++:

``` cpp
Track {
  int32 frame // Probably should be uint32, but I saw a model with negative values
  X value
  if (interpolationType > 1) {
    X inTan
    X outTan
  }
}
```

Where X is a data type. You get the interpolation type from the tracks chunk, which looks like
this:


C++:

``` cpp
TracksChunk {
  uint32 tag
  uint32 tracksCount
  uint32 interpolationType // 0: none
                           // 1: linear
                           // 2: hermite
                           // 3: bezier
  uint32 globalSequenceId
  Track[tracksCount] tracks
}
```

The only difference between all the track types is tag, and the
primitive data type of value/inTan/outTan.

Here is a mapping between track tags and data types, and also their
meaning:


C++:

``` cpp
// Node
KGTR: float[3] translation
KGRT: float[4] rotation
KGSC: float[3] scaling
// Layer
KMTF: uint32 textureId
KMTA: float alpha
KMTE: float emissiveGain
KFC3: float[3] fresnelColor
KFCA: float fresnelAlpha
KFTC: float fresnelTeamColor
// Texture animation
KTAT: float[3] translation
KTAR: float[4] rotation
KTAS: float[3] scaling
//Geoset animation
KGAO: float alpha
KGAC: float[3] color
// Light
KLAS: float attenuationStart
KLAE: float attenuationStartEnd
KLAC: float[3] color
KLAI: float intensity
KLBI: float ambientIntensity
KLBC: float[3] ambientColor
KLAV: float visibility
// Attachment
KATV: float visibility
// Particle emitter
KPEE: float emissionRate
KPEG: float gravity
KPLN: float longitude
KPLT: float latitude
KPEL: float lifespan
KPES: float speed
KPEV: float visibility
// Particle emitter 2
KP2E: float emissionRate
KP2G: float gravity
KP2L: float latitude
KP2S: float speed
KP2V: float visibility
KP2R: float variation
KP2N: float length
KP2W: float width
// Ribbon emitter
KRVS: float visibility
KRHA: float heightAbove
KRHB: float heightBelow
KRAL: float alpha
KRCO: float[3] color
KRTX: uint32 textureSlot
// Camera
KCTR: float[3] translation
KCRL: float rotation
KTTR: float[3] targetTranslation
// Corn emitter
KPPA: float alpha
KPPC: float[3] color
KPPE: float emissionRate
KPPL: float lifespan
KPPS: float speed
KPPV: float visibility
```

So if you are parsing a Node, for example, and you see it has a KGRT
chunk in it, you will parse it as a tracks chunk with the data type of
the tracks being float\[4\].

In order to know if there are indeed keyframe tracks, you can either
track the size of your chunks, and if it\'s smaller than the inclusive
size, it means there are tracks. A second approach is to peek ahead with your reader, and see if you find
a matching tag. This is a little easier, as you don\'t actually have to
check sizes this way. The down side to the second approach is that you must know beforehand
what all the possible track types that are allowed for every kind of
object. This is usually OK, since I\'ve listed them above, but what if there are
actually more we don\'t know about, and you suddenly get a model with
one of them? The parser will fail with weird errors. However, it is unlikely there are more common track types you will
actually encounter, so choose which ever way you like better.

Some notes:

All rotations are expressed with quaternions, except for KCRL
(cameras). While there are 10 face types, generally only type 4 - triangles - is
used. Warcraft 3 animations are specified in milliseconds. That is, if your
code is running at the normal 60 frames per second, then you add 1000/60
to your animation counters every frame.

What do I do with this?

Let\'s say we parsed the whole file. Now what? There is too much information, so I\'ll just go over the basics for now.

Geosets

The main goal is to draw the geosets. This is a standard operation - take the vertices, texture coordinates,
normals, face indices, and call an indexed draw function in your
rendering API. The primitive type can be any primitive type supported by the rendering
API. That being said, it will generally always be 4, or in other words
triangles.

However\...

Materials

\...each geoset references a material. Materials are groups of layers, where each layer gives information such
as the texture used, the layer alpha for translucency, the texture
coordinate set to be used, and much more. As far as graphics code, every layer is a draw call. In pseudo code it could be something along the lines of:


C++:

``` cpp
foreach geoset of geosets:
    foreach layer of materials[geoset.materialId]:
        applyLayer(layer)
        drawGeoset(geoset)
```

An example before we move on - to get team colors working, models
generally have two layers - the base layer is marked as team color and
is completely opaque, and the layer that is rendered above it uses
blending with the desired diffuse texture, such that places in the
diffuse texture that are not opaque will show the team color layer
below.

Layers

So what should applyLayer above do?

textureId The textureId is an index into the model\'s textures list, which you
need to bind into your rendering API.


C++:

``` cpp
bindTexture(textures[layer.textureId])
```

filterMode The filterMode refers to what kind of graphics operation this layer has,
and usually involves blending. Conceptually it\'s very similar to how you\'d combine layers in a 2D
image editing software like Photoshop. Each layer has its own mode, like \"add\", \"blend\", etc., and the
result is the combination of all layers.

If the filter mode is 0, this is a normal opaque draw call.

If the filter mode is 1, this is an alpha-tested opaque draw call with
alpha=0.75. This means that any pixel resulting from this draw call with alpha\<0.75
will not be drawn. This can be achieved either directly via the rendering API, or inside
GPU shaders.

Filter modes 2-6 are for blended draw calls, with different blending
operations. Here\'s code that selects them for WebGL:


JavaScript:

``` {dir="ltr" xf-init="code-block" data-lang="javascript"}
switch (filterMode) {
    // Blended
    case 2:
        blendSrc = gl.SRC_ALPHA;
        blendDst = gl.ONE_MINUS_SRC_ALPHA;
        break;
    // Additive.
    // Note that this isn't pure additive where two colors are added as-is.
    // Warcrft 3 takes also the source alpha into consideration!
    case 3:
        blendSrc = gl.SRC_ALPHA;
        blendDst = gl.ONE;
        break;
    // Referred to as "Add Alpha" by Magos et al., however doesn't seem to be different than Additive.
    case 4:
        blendSrc = gl.SRC_ALPHA;
        blendDst = gl.ONE;
        break;
    // Modulate
    case 5:
        blendSrc = gl.ZERO;
        blendDst = gl.SRC_COLOR;
        break;
    // Modulate 2X
    case 6:
        blendSrc = gl.DST_COLOR;
        blendDst = gl.SRC_COLOR;
        break;
}
```

coordId The coordId field selects a specific coordinate set in the geoset. Generally speaking this will always be 0, and every geoset will always
have exactly one coordinate set. This is due to the fact that no Warcraft 3-related model editing tools
support more than one coordinate set. There are a few models made over the years with multiple coordinate
sets. If you know of one I\'d be happy to get a copy
![:)](data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7 "Smile    :)")

flags The flags field holds more graphics state information, like whether the
draw should be double sided, do depth checks, etc.

The rest\... \...are related to animations.