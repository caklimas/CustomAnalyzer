using System.Composition;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using CustomAnalyzer.Helpers;

namespace CustomAnalyzer.CompilationUnit
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
            var diagnostic = context.Diagnostics.First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => CompilationUnitSyntaxHelpers.CleanupUsingDirectives(context.Document),
                    equivalenceKey: title),
                diagnostic);

            await Task.CompletedTask;
        }
    }
}
