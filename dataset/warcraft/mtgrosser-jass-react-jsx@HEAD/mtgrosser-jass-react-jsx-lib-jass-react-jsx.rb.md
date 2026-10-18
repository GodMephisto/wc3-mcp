---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-react-jsx"
source_path: "lib/jass/react/jsx.rb"
ref: HEAD
---

require 'nodo'

require_relative 'jsx/version'
require_relative 'jsx/compiler'

begin
  require 'sprockets'
rescue LoadError
  # Sprockets not available
end

if defined?(Sprockets)
  require_relative 'jsx/processor'

  Sprockets.register_mime_type 'text/jsx', extensions: %w[.jsx], charset: :unicode
  Sprockets.register_transformer 'text/jsx', 'application/javascript', Jass::React::JSX::Processor
end
