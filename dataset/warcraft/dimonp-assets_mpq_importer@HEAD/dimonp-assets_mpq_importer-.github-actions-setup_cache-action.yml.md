---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: ".github/actions/setup_cache/action.yml"
ref: HEAD
---


name: 'setup_cache'
description: 'sets up the shared cache'
inputs:
  compiler:
    required: true
    type: string
  build_type:
    required: true
    type: string
  generator:
    required: true
    type: string

runs:
  using: "composite"
  steps:
    - name: Cache
      uses: actions/cache@v3
      with:
        # You might want to add .ccache to your cache configuration?
        path: |
          $HOME/.cache/pip
          $HOME/.ccache
          ${{ env.VCPKG_DEFAULT_BINARY_CACHE }}

        key: ${{ runner.os }}-${{ inputs.compiler }}-${{ inputs.build_type }}-${{ inputs.generator }}-${{ hashFiles('**/CMakeLists.txt', './vcpkg.json') }}
        restore-keys: ${{ runner.os }}-${{ inputs.compiler }}-${{ inputs.build_type }}-
