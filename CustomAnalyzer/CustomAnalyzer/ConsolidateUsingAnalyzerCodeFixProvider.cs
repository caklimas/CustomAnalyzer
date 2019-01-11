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

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<UsingDirectiveSyntax>()
                .First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ConsolidateUsingDirectivesAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);

            await Task.CompletedTask;
        }

        private async Task<Document> ConsolidateUsingDirectivesAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken token)
        {
            var leadingTrivia = usingDirective.GetLeadingTrivia()
                .Where(trivia => !trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                .ToList();

            var newNode = usingDirective.WithLeadingTrivia(new SyntaxTriviaList(leadingTrivia));
            var oldRoot = await document.GetSyntaxRootAsync() as CompilationUnitSyntax;
            var newRoot = oldRoot.ReplaceNode(usingDirective, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
