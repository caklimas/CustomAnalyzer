using System.Composition;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using CustomAnalyzer.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CustomAnalyzer.Analyzers.UsingDirective
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RootUsingDirectiveCodeFixProvider)), Shared]
    public class RootUsingDirectiveCodeFixProvider : CodeFixProvider
    {
        private const string title = "Organize using statements";

        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(UsingDirectiveAnalyzer.CompilationUnitDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => CleanupUsingDirectives(context.Document),
                    equivalenceKey: title),
                diagnostic);

            await Task.CompletedTask;
        }

        private async Task<Document> CleanupUsingDirectives(Document document)
        {
            var root = await document.GetSyntaxRootAsync() as CompilationUnitSyntax;
            var usingDirectives = UsingDirectiveHelpers.CleanupUsingDirectives(root.Usings);
            var newRoot = root.WithUsings(new SyntaxList<UsingDirectiveSyntax>(usingDirectives));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
