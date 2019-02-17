using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace CustomAnalyzer.Analyzers.UsingDirective
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UsingDirectiveAnalyzer : DiagnosticAnalyzer
    {
        public const string CompilationUnitDiagnosticId = "CompilationUnit" + nameof(UsingDirectiveAnalyzer);
        public const string NamespaceDiagnosticId = "Namespace" + nameof(UsingDirectiveAnalyzer);

        public static readonly string Title = "Using directives not organized";
        public static readonly string MessageFormat = "USing directives are not organized";
        public static readonly string Description = "Using directives should be consolidated and alphabetized";
        public const string Category = "Using Directives";

        private static DiagnosticDescriptor CompilationUnitRule = new DiagnosticDescriptor(CompilationUnitDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private static DiagnosticDescriptor NamespaceRule = new DiagnosticDescriptor(NamespaceDiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CompilationUnitRule, NamespaceRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeCompilationUnit);
            context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
        }

        private void AnalyzeCompilationUnit(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot() as CompilationUnitSyntax;
            var diagnostics = GetDiagnostics(root.Usings, CompilationUnitRule);
            foreach (var diagnostic in diagnostics)
                context.ReportDiagnostic(diagnostic);
        }

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = context.Node as NamespaceDeclarationSyntax;
            var diagnostics = GetDiagnostics(namespaceDeclaration.Usings, NamespaceRule);
            foreach (var diagnostic in diagnostics)
                context.ReportDiagnostic(diagnostic);
        }

        protected IEnumerable<Diagnostic> GetDiagnostics(
            SyntaxList<UsingDirectiveSyntax> usingDirectives,
            DiagnosticDescriptor rule)
        {
            return AnalyzeUsingDirectiveOrder(usingDirectives, rule)
                .Concat(AnalyzeUsingDirectiveTrivia(usingDirectives, rule))
                .ToList();
        }

        private IEnumerable<Diagnostic> AnalyzeUsingDirectiveOrder(
            SyntaxList<UsingDirectiveSyntax> usingDirectives,
            DiagnosticDescriptor rule)
        {
            return usingDirectives
                .Where((u, i) => usingDirectives.Take(i).Any(n => u.Name.ToString().CompareTo(n.Name.ToString()) == -1))
                .Select(u => Diagnostic.Create(rule, u.GetLocation()))
                .ToList();
        }

        private IEnumerable<Diagnostic> AnalyzeUsingDirectiveTrivia(
            SyntaxList<UsingDirectiveSyntax> usingDirectives,
            DiagnosticDescriptor rule)
        {
            return usingDirectives
                .Where(u => u.GetLeadingTrivia().Any(trivia => trivia.IsKind(SyntaxKind.EndOfLineTrivia)))
                .Select(u => Diagnostic.Create(rule, u.GetLocation()))
                .ToList();
        }
    }
}
