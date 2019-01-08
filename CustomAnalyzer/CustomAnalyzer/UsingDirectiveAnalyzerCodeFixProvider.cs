using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsingDirectiveAnalyzerCodeFixProvider)), Shared]
    public class UsingDirectiveAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Alphabetize using statements";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(UsingDirectiveAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => AlphabetizeUsingDirectivesAsync(context.Document, c),
                    equivalenceKey: title),
                diagnostic);

            await Task.CompletedTask;
        }

        private async Task<Document> AlphabetizeUsingDirectivesAsync(Document document, CancellationToken token)
        {
            var oldRoot = await document.GetSyntaxRootAsync() as CompilationUnitSyntax;
            var sortedUsingDirectives = new SyntaxList<UsingDirectiveSyntax>(oldRoot.Usings.OrderBy(u => u.Name.ToString()).ToList());
            var newRoot = oldRoot.WithUsings(sortedUsingDirectives);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
