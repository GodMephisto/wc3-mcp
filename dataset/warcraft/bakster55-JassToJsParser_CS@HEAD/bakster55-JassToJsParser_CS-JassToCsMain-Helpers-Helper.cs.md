---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Helpers/Helper.cs"
ref: HEAD
---

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Ninject;
using System;
using System.Collections.Generic;
using static JassParser;

namespace JassToCsMain
{
    public class Helper
    {
        public NameGenerator NameGenerator = new NameGenerator(3);

        public Dictionary<string, string> GlobalVariableTypeByName = new Dictionary<string, string>();

        public Dictionary<string, string> NativeGlobalVariableTypeByName = new Dictionary<string, string>();

        public Dictionary<Tuple<string, string>, string> NewNameByOldNameByFuncName = new Dictionary<Tuple<string, string>, string>();

        [Inject]
        public FuncHelper FuncHelper { get; set; }

        public string GetNewName(string oldName, string funcName)
        {
            var key = Tuple.Create(funcName, oldName);

            string newName = NewNameByOldNameByFuncName.GetValueOrDefault(key);
            if (newName == null)
            {
                newName = NameGenerator.GetNext();
                NewNameByOldNameByFuncName.Add(key, newName);
            }

            return newName;
        }

        public string GetNewName(IdContext idContext)
        {
            string id = idContext.ID().GetText();

            if (FuncHelper.FunctionTypeByName.ContainsKey(id) && id != "main")
            {
                return GetNewFuncName(id);
            }

            if (GlobalVariableTypeByName.ContainsKey(id))
            {
                return "gv_" + GetNewName(id, null);
            }


            string funcName = GetParentContext<FuncContext>(idContext)?.funcDeclr()?.id()?.ID()?.GetText();
            if (funcName != null && FuncHelper.LocalVariableTypeByNameByFuncName[funcName].ContainsKey(id))
            {
                return "lv_" + GetNewName(id, funcName);
            }

            return id;
        }

        public string GetNewFuncName(string id)
        {
            return "f_" + GetNewName(id, null);
        }

        //public StringBuilder ReplaceInvalidVariableName(StringBuilder id)
        //{
        //    List<string> reservedKeywords = new List<string>()
        //    {
        //        "do",
        //        "in",
        //        "this",
        //        "class"
        //    };

        //    var reservedKeyword = reservedKeywords.FirstOrDefault(rk => id.ToString() == rk);

        //    if (reservedKeyword != null)
        //    {
        //        return new StringBuilder($"{reservedKeyword}{reservedKeyword}");
        //    }

        //    return id;
        //}

        public void ReplaceBinaryOperator(ParserRuleContext context)
        {
            Dictionary<int, string> replacableOperators = new Dictionary<int, string>()
            {
                { JassLexer.K_AND, "&&" },
                { JassLexer.K_OR, "||" },
                { JassLexer.K_NOT, "!" },
            };

            foreach (var replacableOperator in replacableOperators)
            {
                var token = context.GetToken(replacableOperator.Key, 0);

                if (token != null)
                {
                    var index = context.children.IndexOf(token);
                    context.children[index] = new TerminalNodeImpl(new CommonToken(JassLexer.ID, replacableOperator.Value));
                }
            }
        }

        public void FillGlobalTypes(GlobalVarListContext context)
        {
            FillGlobalTypesInternal(context, GlobalVariableTypeByName);
        }

        public void FillNativeGlobalTypes(GlobalVarListContext context)
        {
            FillGlobalTypesInternal(context, NativeGlobalVariableTypeByName);
        }

        public void FillGlobalTypesInternal(GlobalVarListContext context, IDictionary<string, string> dictionary)
        {
            var varDeclContext = context.varDeclr();

            if (varDeclContext != null)
            {
                foreach (var varDecl in varDeclContext)
                {
                    var type = varDecl.type().GetText();
                    var name = varDecl.id().ID().Symbol.Text;

                    if (!dictionary.ContainsKey(name))
                    {
                        dictionary.Add(name, type);
                    }
                }
            }

            var constDeclContext = context.constDeclr();

            if (constDeclContext != null)
            {
                foreach (var constDecl in constDeclContext)
                {
                    var type = constDecl.type().GetText();
                    var name = constDecl.id().ID().Symbol.Text;

                    dictionary.Add(name, type);
                }
            }
        }

        public bool IsIntegerDivision(ExprContext context)
        {
            if (context.DIV() != null)
            {
                var expressions = context.expr();

                if (expressions != null && expressions.Length > 0 && IsExpressionOfType(expressions[0], "integer") && IsExpressionOfType(expressions[1], "integer"))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsStringConcatenation(ExprContext context)
        {
            if (context.ADD() != null)
            {
                var expressions = context.expr();

                if (expressions != null && expressions.Length > 0 && IsExpressionOfType(expressions[0], "string") && IsExpressionOfType(expressions[1], "string"))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsFourccConcatenation(ExprContext context)
        {
            if (context.ADD() != null)
            {
                var expressions = context.expr();

                if (expressions != null && expressions.Length > 0 && IsExpressionOfType(expressions[0], "fourcc") && IsExpressionOfType(expressions[1], "fourcc"))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsExpressionOfType(ExprContext exprContext, string exprType)
        {
            if (exprType == "integer" && exprContext?.constant()?.intConst() != null)
            {
                return true;
            }

            if (exprType == "fourcc" && exprContext?.constant()?.intConst()?.fourcc() != null)
            {
                return true;
            }

            if (exprType == "string" && exprContext?.constant()?.stringConst() != null)
            {
                return true;
            }

            // Not constant value, try to find type in local or global context
            string objectName = exprContext.id()?.GetText();
            if (objectName != null)
            {
                var localFuncName = GetParentContext<FuncContext>(exprContext)?.funcDeclr()?.id().GetText();
                return GlobalVariableTypeByName.GetValueOrDefault(objectName) == exprType
                    || NativeGlobalVariableTypeByName.GetValueOrDefault(objectName) == exprType
                    || FuncHelper.LocalVariableTypeByNameByFuncName.GetValueOrDefault(localFuncName).GetValueOrDefault(objectName) == exprType;
            }

            // If expression is function call, get func type
            objectName = exprContext.funcCall()?.id()?.GetText();
            if (objectName != null)
            {
                return FuncHelper.FunctionTypeByName.GetValueOrDefault(objectName) == exprType
                    || FuncHelper.NativeFunctionTypeByName.GetValueOrDefault(objectName) == exprType;
            }

            // If expession is array. get its type
            objectName = exprContext.arrayRef()?.id()?.GetText();
            if (objectName != null)
            {
                var localFuncName = GetParentContext<FuncContext>(exprContext)?.funcDeclr()?.id().GetText();
                return GlobalVariableTypeByName.GetValueOrDefault(objectName) == exprType
                    || FuncHelper.LocalVariableTypeByNameByFuncName.GetValueOrDefault(localFuncName).GetValueOrDefault(objectName) == exprType;
            }

            return false;
        }

        public string GetVariableType(IdContext idContext)
        {
            var name = idContext.ID().GetText();

            string type = GlobalVariableTypeByName.GetValueOrDefault(name);
            if (type != null) { return type; }

            var funcName = GetParentContext<FuncContext>(idContext)?.funcDeclr()?.id().GetText();
            return FuncHelper.LocalVariableTypeByNameByFuncName.GetValueOrDefault(funcName).GetValueOrDefault(name);
        }

        public T GetParentContext<T>(ParserRuleContext context) where T : ParserRuleContext
        {
            RuleContext parentContext = context;

            while (parentContext != null && parentContext.GetType() != typeof(T))
            {
                parentContext = parentContext?.Parent;
            }

            return parentContext as T;
        }
    }
}
