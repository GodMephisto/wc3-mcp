---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Helpers/FuncHelper.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using static JassParser;

namespace JassToCsMain
{
    public class FuncHelper
    {
        public Dictionary<string, Dictionary<string, string>> LocalVariableTypeByNameByFuncName = new Dictionary<string, Dictionary<string, string>>();

        public Dictionary<string, string> FunctionTypeByName = new Dictionary<string, string>();

        public Dictionary<string, string> NativeFunctionTypeByName = new Dictionary<string, string>();

        public void FillLocalVariableTypes(FuncContext context)
        {
            var funcName = context.funcDeclr().id().GetText();
            if (LocalVariableTypeByNameByFuncName.ContainsKey(funcName))
            {
                throw new Exception();
            }

            LocalVariableTypeByNameByFuncName.Add(funcName, new Dictionary<string, string>());

            var varDeclContexts = context.localVarList()?.varDeclr();

            if (varDeclContexts != null)
            {
                // Fill function body variables
                foreach (var varDeclrContext in varDeclContexts)
                {
                    var type = varDeclrContext.type().GetText();
                    var name = varDeclrContext.id().GetText();

                    LocalVariableTypeByNameByFuncName[funcName].Add(name, type);
                }
            }

            var paramDeclrContexts = context.funcDeclr()?.paramList()?.paramDeclr();

            if (paramDeclrContexts != null)
            {
                // Fill function parameters
                foreach (var paramDeclrContext in paramDeclrContexts)
                {
                    var type = paramDeclrContext.type().GetText();
                    var name = paramDeclrContext.id().GetText();

                    LocalVariableTypeByNameByFuncName[funcName].Add(name, type);
                }
            }
        }

        public void FillFunctionType(FuncDeclrContext funcDeclrContext)
        {
            string funcName = funcDeclrContext.id().ID().GetText();
            if (!FunctionTypeByName.ContainsKey(funcName))
            {
                FunctionTypeByName.Add(funcName, funcDeclrContext.type()?.GetText());
            }
        }

        public void FillNativeFunctionType(FuncDeclrContext funcDeclrContext)
        {
            string funcName = funcDeclrContext.id().ID().GetText();
            if (!NativeFunctionTypeByName.ContainsKey(funcName))
            {
                NativeFunctionTypeByName.Add(funcName, funcDeclrContext.type()?.GetText());
            }
        }
    }
}
