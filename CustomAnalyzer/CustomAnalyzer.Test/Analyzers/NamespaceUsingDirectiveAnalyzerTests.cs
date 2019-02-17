using CustomAnalyzer.Analyzers;
using CustomAnalyzer.Analyzers.UsingDirective;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TestHelper;

namespace CustomAnalyzer.Test.Analyzers
{
    [TestClass]
    public class RootUsingDiNamespaceUsingDirectiveAnalyzerTestsrectiveAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void ReturnsWarning_WhenDirectivesUnorganized()
        {
            var test = @"namespace ConsoleApplication1
{
    using System.Linq;
    using System;
    class TypeName
    {
    }
}";
            var expected = new DiagnosticResult
            {
                Id = UsingDirectiveAnalyzer.NamespaceDiagnosticId,
                Message = UsingDirectiveAnalyzer.MessageFormat.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line: 4, column: 5)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void ShouldAlphabetizeUsingDirectives_WhenDirectivesUnorganized()
        {
            var test = @"namespace ConsoleApplication1
{
    using System.Linq;

    using System;
    class TypeName
    {
    }
}";

            var fixtest = @"namespace ConsoleApplication1
{
    using System;
    using System.Linq;
    class TypeName
    {
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new NamespaceUsingDirectiveCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UsingDirectiveAnalyzer();
        }
    }
}
