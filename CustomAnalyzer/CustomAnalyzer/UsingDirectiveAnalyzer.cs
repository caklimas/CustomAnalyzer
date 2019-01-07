using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CustomAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UsingDirectiveAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UsingDirectiveAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        public static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        public const string Category = "Using Directives";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxTreeAction(AnalyzeUsingDirectives);
        }
        
        private void AnalyzeUsingDirectives(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot();
            var usingDirectives = root.ChildNodes()
                .OfType<UsingDirectiveSyntax>()
                .ToList();
            
            for (var i = 0; i < usingDirectives.Count; i++)
            {
                var usingDirective = usingDirectives[i];
                if (usingDirectives.Take(i).Any(n => usingDirective.Name.ToString().CompareTo(n.Name.ToString()) == -1))
                {
                    var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
