using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using CustomAnalyzer.NamespaceDeclaration;

namespace CustomAnalyzer.Test.NamespaceDeclaration
{
    [TestClass]
    public class AlphabetizeUsingAnalyzerTests : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void ReturnsWarning_WhenDirectivesOutOfOrder()
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
                Id = AlphabetizeUsingAnalyzer.DiagnosticId,
                Message = AlphabetizeUsingAnalyzer.MessageFormat.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line: 4, column: 5)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void ShouldAlphabetizeUsingDirectives_WhenDirectivesOutOfOrder()
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
            return new AlphabetizeUsingAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AlphabetizeUsingAnalyzer();
        }
    }
}
