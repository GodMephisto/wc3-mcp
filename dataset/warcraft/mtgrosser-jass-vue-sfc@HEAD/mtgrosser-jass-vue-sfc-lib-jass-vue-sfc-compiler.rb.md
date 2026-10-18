---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-vue-sfc"
source_path: "lib/jass/vue/sfc/compiler.rb"
ref: HEAD
---

require 'digest/md5'

class Jass::Vue::SFC::Compiler < Nodo::Core
  const :COMP_IDENTIFIER, '__sfc__'
  
  require compiler: '@vue/compiler-sfc'

  class_function def compile(source, filename)
    filename = File.basename(filename)
    id = Digest::MD5.hexdigest(filename)[0..7]
    compile_component(source, filename, id)
  end
  
  function :compile_component, <<~'JS'
    (source, filename, id) => {
      let code = '';
      nodo.debug(`Compiling component ${filename}`);
      const { errors, descriptor } = compiler.parse(source, { filename, sourceMap: true });
  
      const [clientScript, bindings] = compile_script(descriptor, id);
      if (clientScript) {
        code += clientScript;
      }
      const clientTemplate = compile_template(descriptor, id, bindings);
      if (clientTemplate) {
        code += clientTemplate;
      }
      
      return code;
    }
  JS

  function :compile_script, <<~'JS'
    (descriptor, id) => {
      if (descriptor.script || descriptor.scriptSetup) {
        try {
          const compiledScript = compiler.compileScript(descriptor, {
              id,
              refSugar: true,
              inlineTemplate: true,
              templateOptions: { ssrCssVars: descriptor.cssVars }
          });
          let code = '';
          if (compiledScript.bindings) {
            code += `\n/* Analyzed bindings: ${JSON.stringify(compiledScript.bindings, null, 2)} */`;
          }
          code += `\n` + compiler.rewriteDefault(compiledScript.content, COMP_IDENTIFIER);
          return [code, compiledScript.bindings];
        } catch (e) {
          e.fileName = descriptor.filename;
          throw e;
        }
      } else {
        return [`\nconst ${COMP_IDENTIFIER} = {}`, undefined];
      }
    }
  JS

  function :compile_template, <<~'JS'
    (descriptor, id, bindingMetadata) => {
      if (descriptor.template && !descriptor.scriptSetup) {
        const templateResult = compiler.compileTemplate({
          source: descriptor.template.content,
          filename: descriptor.filename,
          id: id,
          scoped: descriptor.styles.some(s => s.scoped),
          slotted: descriptor.slotted,
          ssr: false,
          ssrCssVars: descriptor.cssVars,
          isProd: 'production' == process.env.NODE_ENV,
          compilerOptions: { bindingMetadata }
        });
        if (templateResult.errors.length) {
          const error = templateResult.errors[0];
          error.fileName = descriptor.filename;
          throw error;
        }
        let code = `${templateResult.code.replace(/\nexport (function|const) (render|ssrRender)/, '$1 render')}\n`;
        code += `${COMP_IDENTIFIER}.__file = ${JSON.stringify(descriptor.filename)}\n`;
        code += `${COMP_IDENTIFIER}.render = render\n`;
        code += `export default ${COMP_IDENTIFIER}\n`;
        
        return code;
      }
    }
  JS

end
