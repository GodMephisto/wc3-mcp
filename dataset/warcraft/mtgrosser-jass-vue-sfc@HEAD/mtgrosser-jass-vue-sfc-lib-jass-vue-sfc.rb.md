---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-vue-sfc"
source_path: "lib/jass/vue/sfc.rb"
ref: HEAD
---

require 'nodo'

require_relative 'sfc/version'
require_relative 'sfc/compiler'

begin
  require 'sprockets'
rescue LoadError
  # Sprockets not available
end

if defined?(Sprockets)
  require_relative 'sfc/processor'
  Sprockets.register_mime_type 'text/vue-sfc', extensions: %w[.vue], charset: :unicode
  Sprockets.register_transformer 'text/vue-sfc', 'application/javascript', Jass::Vue::SFC::Processor
end
