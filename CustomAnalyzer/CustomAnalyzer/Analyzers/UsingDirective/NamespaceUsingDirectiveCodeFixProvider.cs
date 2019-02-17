using System.Composition;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using CustomAnalyzer.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace CustomAnalyzer.Analyzers.UsingDirective
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamespaceUsingDirectiveCodeFixProvider)), Shared]
    public class NamespaceUsingDirectiveCodeFixProvider : CodeFixProvider
    {
        private const string title = "Organize using statements";

        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(UsingDirectiveAnalyzer.NamespaceDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync();
            var diagnostic = context.Diagnostics.FirstOrDefault();
            if (diagnostic == null) return;

            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start)
                .Parent.AncestorsAndSelf()
                .OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

            if (declaration == null) return;

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => CleanupUsingDirectives(context.Document, declaration),
                    equivalenceKey: title),
                diagnostic);

            await Task.CompletedTask;
        }

        private async Task<Document> CleanupUsingDirectives(
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
