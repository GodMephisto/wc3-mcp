---
type: vendor-doc
source_repo: "https://github.com/qq200774491/war3map_w3r"
source_path: "xmake.lua"
ref: HEAD
---

set_project("war3map_w3r")
set_version("1.0.0")

set_languages("c++20")
set_warnings("all")

add_rules("mode.debug", "mode.release")

target("war3map_w3r")
set_kind("binary")
add_files("src/*.cpp")
add_includedirs("src")

if is_plat("windows") then
    add_cxflags("/utf-8", { force = true })
end
