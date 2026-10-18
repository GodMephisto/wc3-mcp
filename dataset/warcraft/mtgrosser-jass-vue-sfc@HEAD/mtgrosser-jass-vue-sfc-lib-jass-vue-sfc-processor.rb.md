---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-vue-sfc"
source_path: "lib/jass/vue/sfc/processor.rb"
ref: HEAD
---

module Jass::Vue::SFC::Processor
  VERSION = '1'

  class << self
    
    def cache_key
      @cache_key ||= "#{name}:#{VERSION}".freeze
    end
    
    def call(input)
      data = input[:data]

      js, map = input[:cache].fetch([self.cache_key, data]) do
        result = Jass::Vue::SFC::Compiler.compile(data, input[:filename])
        [result, nil]
      end

      #map = SourceMapUtils.format_source_map(map, input)
      #map = SourceMapUtils.combine_source_maps(input[:metadata][:map], map)

      { data: js } #, map: map }
    end
  
  end
end
