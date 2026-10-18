---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-react-jsx"
source_path: "lib/jass/react/jsx/processor.rb"
ref: HEAD
---

module Jass::React::JSX::Processor
  VERSION = '1'

  class << self
    
    def cache_key
      @cache_key ||= [name, VERSION].freeze
    end
    
    def call(input)
      data = input[:data]

      js, map = input[:cache].fetch(cache_key + [input[:filename]] + [data]) do
        result = Jass::React::JSX::Compiler.compile(data, input[:filename])
        result.values_at('code', 'map')
      end

      #map = Sprockets::SourceMapUtils.format_source_map(map, input)
      #map = Sprockets::SourceMapUtils.combine_source_maps(input[:metadata][:map], map)

      { data: js } #, map: map }
    end
  
  end
end
