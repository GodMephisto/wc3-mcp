---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-rollup"
source_path: "lib/jass/rollup/directive_processor.rb"
ref: HEAD
---

module Jass
  module Rollup
    class DirectiveProcessor
      DIRECTIVES = %w[rollup rollup_esm require_npm].freeze
      
      class << self
        def instance
          @instance ||= new
        end
        
        def call(input)
          instance.call(input)
        end
      end

      def call(input)
        data = "#{input[:data] || ''}"
        data.gsub!(%r{^//=\s*(?<directive>#{DIRECTIVES.join('|')})\s+(?<args>.+)$}) do
          match = Regexp.last_match
          process_matching_directive(match[:directive], match[:args].squish.split(' '))
        end
        { data: data }
      end
      
      def process_matching_directive(name, args)
        "#{send("process_#{name}_directive", *args)}\n"
      rescue => e
        e.set_backtrace(["#{@filename}:0"] + e.backtrace)
        raise e
      end
      
      def process_rollup_esm_directive(mod)
        Compiler.compile_esm(mod).fetch('code')
      end

      def process_rollup_directive(entry, *args)
        Compiler.compile_entry(entry).fetch('code')
      end
      
      def process_require_npm_directive(path, *args)
        File.read(File.expand_path(path, Nodo.modules_root))
      end
    end
  end
end
