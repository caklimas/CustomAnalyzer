using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CustomAnalyzer.Helpers
{
    public static class UsingDirectiveHelpers
    {
        public static UsingDirectiveSyntax ConsolidateUsingDirective(UsingDirectiveSyntax usingDirective)
        {
            var leadingTrivia = usingDirective.GetLeadingTrivia()
                .Where(trivia => !trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                .ToList();

            return usingDirective.WithLeadingTrivia(new SyntaxTriviaList(leadingTrivia));
        }
    }
}
