---
type: vendor-doc
source_repo: "https://github.com/warcraft-iii/war3map.cpp"
source_path: "xmake.lua"
ref: HEAD
---



set_project('war3map.cpp')

    set_version('1.0.0')
    set_languages('cxx17', is_plat('windows') and 'c89' or 'c99')
    set_defaultplat('windows')
    set_defaultarchs('x86')
    add_rules('mode.debug', 'mode.release')
    add_rules('plugin.vsxmake.autoupdate')
    set_runtimes(is_mode('debug') and 'MTd' or 'MT')
    set_config('vs_sdkver', '10.0.18362.0')
    set_symbols('debug')

    if is_mode('debug') then
        add_defines('_DEBUG')
    else
        add_defines('NDEBUG')
    end
    if is_plat('windows') then
        add_defines({ 'WIN32', '_WINDOWS', 'NOMINMAX' })
    end

    includes('src')
