---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Helpers/Parser.cs"
ref: HEAD
---

using Antlr4.Runtime;
using Ninject;
using System.Text;
using static JassParser;

namespace JassToCsMain
{
    public class Parser
    {
        [Inject]
        public JassVisitor JassVisitor { get; set; }

        [Inject]
        public JassNativeParserVisitor JassNativeParserVisitor { get; set; }

        public string Parse(string path)
        {
            ParseNativeFiles();

            return ParseInternal(path, JassVisitor);
        }

        public void ParseNativeFiles()
        {
            string baseJassNativePath = @"E:\JassToCsMain\JassToCsMain\War3Map";

            ParseNativeFile($@"{baseJassNativePath}\common.j");
            ParseNativeFile($@"{baseJassNativePath}\blizzard.j");
        }

        public string ParseNativeFile(string path)
        {
            return ParseInternal(path, JassNativeParserVisitor);
        }

        public FileContext GetFileContext(string path)
        {
            FileContext tree;
            using (new TimeTracker("parsing"))
            {
                ICharStream charStream = CharStreams.fromPath(path, Encoding.Default);

                var lexer = new JassLexer(charStream);
                var tokens = new CommonTokenStream(lexer);
                var parser = new JassParser(tokens);
                parser.BuildParseTree = true;

                tree = parser.file();
            }

            return tree;
        }

        public string ParseInternal(string path, JassParserVisitor jassParserVisitor)
        {
            FileContext tree = GetFileContext(path);

            return ParseInternal(tree, jassParserVisitor);
        }

        public string ParseInternal(FileContext fileContext, JassParserVisitor jassParserVisitor)
        {
            string content;
            using (new TimeTracker("visiting"))
            {
                content = jassParserVisitor.Visit(fileContext).ToString();
            }

            return content;
        }
    }
}
