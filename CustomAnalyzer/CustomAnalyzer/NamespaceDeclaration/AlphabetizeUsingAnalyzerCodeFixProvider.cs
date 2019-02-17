using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using CustomAnalyzer.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;

namespace CustomAnalyzer.NamespaceDeclaration
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AlphabetizeUsingAnalyzerCodeFixProvider)), Shared]
    public class AlphabetizeUsingAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Alphabetize using statements";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(AlphabetizeUsingAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
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
                    createChangedDocument: c =>
                        NamespaceDeclarationHelpers.CleanupUsingDirectives(context.Document, declaration),
                    equivalenceKey: title),
                diagnostic);

            await Task.CompletedTask;
        }
    }
}
