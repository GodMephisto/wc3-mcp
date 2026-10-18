---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Main/JassParserVisitor.cs"
ref: HEAD
---

using Antlr4.Runtime.Tree;
using System;
using System.Text;

namespace JassToCsMain
{
    public class JassParserVisitor : JassParserBaseVisitor<StringBuilder>
    {
        #region Inherited

        public override StringBuilder Visit(IParseTree tree)
        {
            if (tree != null)
            {
                return tree.Accept(this);
            }

            return new StringBuilder(string.Empty);
        }

        public override StringBuilder VisitChildren(IRuleNode node)
        {
            if (node != null)
            {
                return base.VisitChildren(node);
            }

            return new StringBuilder(string.Empty);
        }

        public override StringBuilder VisitTerminal(ITerminalNode node)
        {
            return new StringBuilder(node.Symbol.Text);
        }

        protected override StringBuilder AggregateResult(StringBuilder aggregate, StringBuilder nextResult)
        {
            return aggregate != null
                ? aggregate.Append(nextResult)
                : nextResult;
        }

        public override StringBuilder VisitErrorNode(IErrorNode node)
        {
            throw new Exception();
        }

        #endregion
    }
}
