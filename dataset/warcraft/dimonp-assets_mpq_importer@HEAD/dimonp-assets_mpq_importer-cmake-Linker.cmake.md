---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "cmake/Linker.cmake"
ref: HEAD
---

macro(assets_mpq_importer_configure_linker project_name)
  set(assets_mpq_importer_USER_LINKER_OPTION
    "DEFAULT"
      CACHE STRING "Linker to be used")
    set(assets_mpq_importer_USER_LINKER_OPTION_VALUES "DEFAULT" "SYSTEM" "LLD" "GOLD" "BFD" "MOLD" "SOLD" "APPLE_CLASSIC" "MSVC")
  set_property(CACHE assets_mpq_importer_USER_LINKER_OPTION PROPERTY STRINGS ${assets_mpq_importer_USER_LINKER_OPTION_VALUES})
  list(
    FIND
    assets_mpq_importer_USER_LINKER_OPTION_VALUES
    ${assets_mpq_importer_USER_LINKER_OPTION}
    assets_mpq_importer_USER_LINKER_OPTION_INDEX)

  if(${assets_mpq_importer_USER_LINKER_OPTION_INDEX} EQUAL -1)
    message(
      STATUS
        "Using custom linker: '${assets_mpq_importer_USER_LINKER_OPTION}', explicitly supported entries are ${assets_mpq_importer_USER_LINKER_OPTION_VALUES}")
  endif()

  set_target_properties(${project_name} PROPERTIES LINKER_TYPE "${assets_mpq_importer_USER_LINKER_OPTION}")
endmacro()
