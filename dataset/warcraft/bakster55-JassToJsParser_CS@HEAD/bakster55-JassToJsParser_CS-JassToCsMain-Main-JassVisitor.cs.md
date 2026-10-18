---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Main/JassVisitor.cs"
ref: HEAD
---

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static JassParser;

namespace JassToCsMain
{
    public class JassVisitor : JassParserVisitor
    {
        [Inject]
        public Helper Helper { get; set; }

        [Inject]
        public FuncHelper FuncHelper { get; set; }

        public bool ReplaceNames { get; set; } = false;

        #region Custom code

        public StringBuilder VisitChildrens(IList<IParseTree> nodes, string separator = "")
        {
            if (nodes != null)
            {
                StringBuilder stringBuilder = new StringBuilder(string.Empty);

                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    var node = nodes[i];
                    stringBuilder.Append($"{this.Visit(node)}{separator}");
                }

                if (nodes.Count > 0)
                {
                    stringBuilder.Append($"{this.Visit(nodes[nodes.Count - 1])}");
                }

                return stringBuilder;
            }

            return new StringBuilder(string.Empty);
        }

        public override StringBuilder VisitArgs([NotNull] JassParser.ArgsContext context)
        {
            return base.VisitArgs(context);
        }

        public override StringBuilder VisitArrayRef([NotNull] JassParser.ArrayRefContext context)
        {
            if (Helper.GetVariableType(context.id()) == "integer" && context.Parent.GetType() == typeof(ExprContext))
            {
                // jass integer arrays have default value 0 instead of js undefined value if element not found
                return new StringBuilder($"{this.Visit(context.id())}.getInt({this.Visit(context.expr())})");
            }

            return base.VisitArrayRef(context);
        }

        public override StringBuilder VisitBoolConst([NotNull] JassParser.BoolConstContext context)
        {
            return base.VisitBoolConst(context);
        }

        public override StringBuilder VisitCall([NotNull] JassParser.CallContext context)
        {
            string args = this.Visit(context.args()).ToString();
            if (context.id().GetText() == "ExecuteFunc")
            {
                args = args.Replace("\"", "");
                // args is the name of the function and its a string
                args = $"\"{Helper.GetNewFuncName(args)}\"";
            }

            return new StringBuilder($"{this.Visit(context.id())}({args})");
        }

        public override StringBuilder VisitConstant([NotNull] JassParser.ConstantContext context)
        {
            return base.VisitConstant(context);
        }

        public override StringBuilder VisitDebug([NotNull] JassParser.DebugContext context)
        {
            return base.VisitDebug(context);
        }

        public override StringBuilder VisitDeclr([NotNull] JassParser.DeclrContext context)
        {
            return new StringBuilder($"{base.VisitDeclr(context)}{Environment.NewLine}");
        }

        public override StringBuilder VisitElseClause([NotNull] JassParser.ElseClauseContext context)
        {
            var statementList = this.Visit(context.statementList());
            if (context.K_ELSEIF() != null)
            {
                var expr = this.Visit(context.expr());
                var elseClause = this.Visit(context.elseClause());

                return new StringBuilder($"{Environment.NewLine}else {{{Environment.NewLine}if({expr}) {{{Environment.NewLine}{statementList}}}{elseClause}{Environment.NewLine}}}");
            }
            else
            {
                return new StringBuilder($"{Environment.NewLine}else {{{Environment.NewLine}{statementList}}}");
            }
        }

        public override StringBuilder VisitExitwhen([NotNull] JassParser.ExitwhenContext context)
        {
            return new StringBuilder($"if ({this.Visit(context.expr())}) {{ break; }}");
        }

        public override StringBuilder VisitExpr([NotNull] JassParser.ExprContext context)
        {
            Helper.ReplaceBinaryOperator(context);

            if (Helper.IsIntegerDivision(context))
            {
                return new StringBuilder($"R2I({base.VisitExpr(context)})");
            }

            if (Helper.IsStringConcatenation(context))
            {
                return new StringBuilder($"S2S({base.VisitExpr(context)})");
            }

            // Optional
            if (Helper.IsFourccConcatenation(context))
            {
                var expressions = context.expr();
                var fourcc1 = expressions[0]?.constant()?.intConst()?.fourcc().FOURCC().Symbol.Text;
                var fourcc2 = expressions[1]?.constant()?.intConst()?.fourcc().FOURCC().Symbol.Text;

                if (fourcc1 == "'�=ū'")
                {

                }

                // 0 and 5 are parenteses
                byte[] bytes1 = Encoding.Default.GetBytes(fourcc1);
                int num1 = ((sbyte)bytes1[1]) * 256 * 256 * 256 + ((sbyte)bytes1[2]) * 256 * 256 + ((sbyte)bytes1[3]) * 256 + ((sbyte)bytes1[4]);

                byte[] bytes2 = Encoding.Default.GetBytes(fourcc2);
                int num2 = ((sbyte)bytes2[1]) * 256 * 256 * 256 + ((sbyte)bytes2[2]) * 256 * 256 + ((sbyte)bytes2[3]) * 256 + ((sbyte)bytes2[4]);
                int num3 = num1 + num2;

                return new StringBuilder(num3.ToString());
            }

            return base.VisitExpr(context);
        }

        public override StringBuilder VisitFile([NotNull] JassParser.FileContext context)
        {
            return base.VisitFile(context);
        }

        public override StringBuilder VisitFunc([NotNull] JassParser.FuncContext context)
        {
            FuncHelper.FillLocalVariableTypes(context);

            var funcDeclr = this.Visit(context.funcDeclr());
            var localVarList = this.Visit(context.localVarList());
            var statementList = this.Visit(context.statementList());

            return new StringBuilder($"function {funcDeclr}{localVarList}{statementList}}}{Environment.NewLine}");
        }

        public override StringBuilder VisitFuncCall([NotNull] JassParser.FuncCallContext context)
        {
            return base.VisitFuncCall(context);
        }

        public override StringBuilder VisitFuncDeclr([NotNull] JassParser.FuncDeclrContext context)
        {
            FuncHelper.FillFunctionType(context);

            return new StringBuilder($"{this.Visit(context.id())}({this.Visit(context.paramList())}) {{{Environment.NewLine}");
        }

        public override StringBuilder VisitParamList([NotNull] JassParser.ParamListContext context)
        {
            return this.VisitChildrens(context.paramDeclr(), ",");
        }

        public override StringBuilder VisitParamDeclr([NotNull] ParamDeclrContext context)
        {
            return base.Visit(context.id());
        }

        public override StringBuilder VisitFuncRef([NotNull] JassParser.FuncRefContext context)
        {
            return this.Visit(context.id());
        }

        public override StringBuilder VisitGlobals([NotNull] JassParser.GlobalsContext context)
        {
            return this.Visit(context.globalVarList());
        }

        public override StringBuilder VisitGlobalVarList([NotNull] JassParser.GlobalVarListContext context)
        {
            Helper.FillGlobalTypes(context);

            return base.VisitGlobalVarList(context);
        }

        public override StringBuilder VisitConstDeclr([NotNull] ConstDeclrContext context)
        {
            if (context.K_CONSTANT() != null)
            {
                context.children.RemoveAt(0);
            }

            return new StringBuilder($"{base.VisitConstDeclr(context)}{Environment.NewLine}");
        }

        public override StringBuilder VisitIfthenelse([NotNull] JassParser.IfthenelseContext context)
        {
            return new StringBuilder($"if ({this.Visit(context.expr())}) {{{Environment.NewLine}{this.Visit(context.statementList())}}}{this.Visit(context.elseClause())}");
        }

        public override StringBuilder VisitIntConst([NotNull] JassParser.IntConstContext context)
        {
            return base.VisitIntConst(context);
        }

        public override StringBuilder VisitFourcc([NotNull] FourccContext context)
        {
            string fourcc = base.VisitFourcc(context).ToString();

            // Replace string with number
            int bracketsLength = 2;
            if (fourcc.Length - bracketsLength == 1)
            {
                return new StringBuilder(Encoding.Default.GetBytes(fourcc)[1].ToString());
            }

            return new StringBuilder(fourcc);
        }

        public override StringBuilder VisitLocalVarList([NotNull] JassParser.LocalVarListContext context)
        {
            return this.VisitChildrens(context.varDeclr());
        }

        public override StringBuilder VisitLoop([NotNull] JassParser.LoopContext context)
        {
            return new StringBuilder($"do {{{Environment.NewLine}{this.Visit(context.statementList())}}} while(true)");
        }

        public override StringBuilder VisitNativeFunc([NotNull] JassParser.NativeFuncContext context)
        {
            return new StringBuilder($"function {this.Visit(context.funcDeclr())}}}");
        }

        public override StringBuilder VisitParens([NotNull] JassParser.ParensContext context)
        {
            return base.VisitParens(context);
        }

        public override StringBuilder VisitReturnStat([NotNull] JassParser.ReturnStatContext context)
        {
            return this.VisitChildrens(context.children, " ");
        }

        public override StringBuilder VisitSet([NotNull] JassParser.SetContext context)
        {
            // Remove set keyword
            context.children?.RemoveAt(0);

            return base.VisitSet(context);
        }

        public override StringBuilder VisitStatement([NotNull] JassParser.StatementContext context)
        {
            return new StringBuilder($"{base.VisitStatement(context)}{Environment.NewLine}");
        }

        public override StringBuilder VisitStatementList([NotNull] JassParser.StatementListContext context)
        {
            return base.VisitStatementList(context);
        }

        public override StringBuilder VisitType([NotNull] JassParser.TypeContext context)
        {
            return new StringBuilder("var ");
        }

        public override StringBuilder VisitTypedef([NotNull] JassParser.TypedefContext context)
        {
            return new StringBuilder(string.Empty);
        }

        public override StringBuilder VisitVarDeclr([NotNull] JassParser.VarDeclrContext context)
        {
            if (context.K_ARRAY() != null)
            {
                // Remove array keyword
                context.children?.RemoveAt(1);

                return new StringBuilder($"{base.VisitVarDeclr(context)} = [];{Environment.NewLine}");
            }

            return new StringBuilder($"{base.VisitVarDeclr(context)};{Environment.NewLine}");
        }

        public override StringBuilder VisitStringConst([NotNull] StringConstContext context)
        {
            string stringConst = base.VisitStringConst(context).ToString();

            // JS does not allow lune breaks in string
            Regex regex = new Regex("[\r\n]");
            if (regex.IsMatch(stringConst))
            {
                return new StringBuilder(regex.Replace(stringConst, string.Empty));
            }

            return new StringBuilder(stringConst);
        }

        public override StringBuilder VisitId([NotNull] IdContext context)
        {
            if (ReplaceNames)
            {
                return new StringBuilder(Helper.GetNewName(context));
            }
            else
            {
                return base.VisitId(context);
            }
        }

        public override StringBuilder VisitDecimal([NotNull] DecimalContext context)
        {
            string number = base.VisitDecimal(context).ToString();

            return new StringBuilder($"{NumberConvertHelper.GetDecimalOrFourcc(number)}");
        }

        public override StringBuilder VisitHex([NotNull] HexContext context)
        {
            string hex = base.VisitHex(context).ToString();
            int number = NumberConvertHelper.HexToDecimal(hex);

            return new StringBuilder($"{NumberConvertHelper.GetDecimalOrFourcc(number)}");
        }

        #endregion
    }
}
