using System;
using System.Composition;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace CustomAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConsolidateUsingAnalyzerCodeFixProvider)), Shared]
    public class ConsolidateUsingAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Consolidate using statements";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ConsolidateUsingAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostics = context.Diagnostics;

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ConsolidateUsingDirectivesAsync(context.Document, diagnostics, c),
                    equivalenceKey: title),
                diagnostics);

            await Task.CompletedTask;
        }

        private async Task<Document> ConsolidateUsingDirectivesAsync(
            Document document, 
            IEnumerable<Diagnostic> diagnostics, 
            CancellationToken token)
        {
            var root = await document.GetSyntaxRootAsync();
            var usingDirectives = diagnostics
                .Select(d => d.Location.SourceSpan)
                .Select(s => root.FindToken(s.Start)
                    .Parent.AncestorsAndSelf()
                    .OfType<UsingDirectiveSyntax>()
                    .First())
                .ToList();

            var newRoot = root;
            foreach(var usingDirective in usingDirectives)
                newRoot = root.ReplaceNode(usingDirective, ConsolidateUsingDirective(usingDirective));

            return document.WithSyntaxRoot(newRoot);
        }

        private UsingDirectiveSyntax ConsolidateUsingDirective(UsingDirectiveSyntax usingDirective)
        {
            var leadingTrivia = usingDirective.GetLeadingTrivia()
                .Where(trivia => !trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                .ToList();

            return usingDirective.WithLeadingTrivia(new SyntaxTriviaList(leadingTrivia));
        }
    }
}
