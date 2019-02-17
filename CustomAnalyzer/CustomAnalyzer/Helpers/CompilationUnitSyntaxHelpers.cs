using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;

namespace CustomAnalyzer.Helpers
{
    public static class CompilationUnitSyntaxHelpers
    {
        public static async Task<Document> CleanupUsingDirectives(Document document)
        {
            var root = await document.GetSyntaxRootAsync() as CompilationUnitSyntax;
            var usingDirectives = UsingDirectiveHelpers.CleanupUsingDirectives(root.Usings);
            var newRoot = root.WithUsings(new SyntaxList<UsingDirectiveSyntax>(usingDirectives));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
