using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomAnalyzer.Helpers
{
    public static class NamespaceDeclarationHelpers
    {
        public static async Task<Document> CleanupUsingDirectives(
            Document document,
            NamespaceDeclarationSyntax namespaceDeclaration)
        {
            var usingDirectives = UsingDirectiveHelpers.CleanupUsingDirectives(namespaceDeclaration.Usings);
            var usings = new SyntaxList<UsingDirectiveSyntax>(usingDirectives);
            var newNamespaceDeclaration = namespaceDeclaration.WithUsings(usings);
            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(namespaceDeclaration, newNamespaceDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
