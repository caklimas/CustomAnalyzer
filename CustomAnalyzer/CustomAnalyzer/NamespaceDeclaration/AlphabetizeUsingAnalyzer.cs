using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CustomAnalyzer.NamespaceDeclaration
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AlphabetizeUsingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AlphabetizeUsingAnalyzer";
        public static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AlphabetizeAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AlphabetizeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AlphabetizeAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        public const string Category = "Using Directives";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.NamespaceDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = context.Node as NamespaceDeclarationSyntax;
            var usingDirectives = namespaceDeclaration.Usings.ToList();

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
