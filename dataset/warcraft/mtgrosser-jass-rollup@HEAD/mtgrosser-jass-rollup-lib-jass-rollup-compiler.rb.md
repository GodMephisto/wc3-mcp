---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-rollup"
source_path: "lib/jass/rollup/compiler.rb"
ref: HEAD
---

class Jass::Rollup::Compiler < Nodo::Core
  require :rollup,
          commonjs: '@rollup/plugin-commonjs', 
          nodeResolve: '@rollup/plugin-node-resolve'

  # workaround for https://github.com/rollup/rollup/issues/4251
  script <<~'JS'
    const loadConfigFile = require(path.join(path.dirname(require.resolve('rollup')), 'loadConfigFile'));
  JS

  class_function def compile(config)
    result = bundle(config)
    result.fetch('output').first.slice('code', 'map')
  end
  
  class_function def compile_esm(mod)
    result = bundle_esm(mod)
    result.fetch('output').first.slice('code', 'map')
  end
  
  class_function def compile_entry(entry)
    result = bundle_entry(entry)
    result.fetch('output').first.slice('code', 'map')
  end
  
  function :default_plugins, <<~'JS'
    () => [
      nodeResolve.nodeResolve({ moduleDirectories: [process.env.NODE_PATH] }),
      commonjs({ requireReturnsDefault: 'namespace', defaultIsModuleExports: true })
    ]
  JS
  
  function :bundle_esm, <<~'JS'
    async (mod) => {
      const entry = require.resolve(mod);
      return await bundle_entry(entry);
    }
  JS
  
  function :bundle, <<~'JS'
    async (config) => {
      const { options, warnings } = await loadConfigFile(config, { format: 'es' });
      
      warnings.flush();
      
      let inputOptions = options[0];
      let outputOptions = options[0].output[0];
      let inputPlugins = inputOptions.plugins;

      if (inputPlugins && inputPlugins.length == 1 && inputPlugins[0].name == 'stdin') {
        inputPlugins.push(...default_plugins().reverse());
      }

      const bundle = await rollup.rollup(inputOptions);
      return await bundle.generate(outputOptions);
    }
  JS
  
  function :bundle_entry, <<~'JS'
    async (entry) => {
      const inputOptions = {
        input: entry,
        plugins: default_plugins()
      };
      const outputOptions = {
        format: 'es',
        exports: 'named'
      };
      const bundle = await rollup.rollup(inputOptions);
      return await bundle.generate(outputOptions);
    }
  JS

end
