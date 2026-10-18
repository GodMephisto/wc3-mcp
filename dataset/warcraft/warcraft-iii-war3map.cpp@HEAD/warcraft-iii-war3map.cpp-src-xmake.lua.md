---
type: vendor-doc
source_repo: "https://github.com/warcraft-iii/war3map.cpp"
source_path: "src/xmake.lua"
ref: HEAD
---


target("war3map")
    set_kind('shared')
    add_headerfiles('**.h')
    add_files('**.cpp')
    add_includedirs(path.join(os.projectdir(), 'include/war3map.cpp/'))
    add_values('charset', 'MBCS')
