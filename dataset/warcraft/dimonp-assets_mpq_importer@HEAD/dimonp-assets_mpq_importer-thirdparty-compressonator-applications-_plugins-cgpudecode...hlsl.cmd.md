---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "thirdparty/compressonator/applications/_plugins/cgpudecode/vulkan/hlsl.cmd"
ref: HEAD
---

fxc ddsview.fx /nologo /EVS /Tvs_4_1 /Fhdxshaders\vs.h
fxc ddsview.fx /nologo /EPS_1D /Tps_4_1 /Fhdxshaders\ps1D.h
fxc ddsview.fx /nologo /EPS_1DArray /Tps_4_1 /Fhdxshaders\ps1Darray.h
fxc ddsview.fx /nologo /EPS_2D /Tps_4_1 /Fhdxshaders\ps2D.h
fxc ddsview.fx /nologo /EPS_2DArray /Tps_4_1 /Fhdxshaders\ps2Darray.h
fxc ddsview.fx /nologo /EPS_3D /Tps_4_1 /Fhdxshaders\ps3D.h
fxc ddsview.fx /nologo /EPS_Cube /Tps_4_1 /Fhdxshaders\psCube.h

