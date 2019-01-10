using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CustomAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConsolidateUsingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(ConsolidateUsingAnalyzer);

        public static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ConsolidateAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ConsolidateAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ConsolidateAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        public const string Category = "Using Directives";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
        }

        private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
        {
            var usingDirective = context.Node as UsingDirectiveSyntax;
            if (!usingDirective.GetLeadingTrivia().Any())
                return;

            var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
