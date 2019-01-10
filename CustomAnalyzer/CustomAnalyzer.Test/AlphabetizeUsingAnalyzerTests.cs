using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using CustomAnalyzer;

namespace CustomAnalyzer.Test
{
    [TestClass]
    public class AlphabetizeUsingAnalyzerTests : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void ReturnsWarning_WhenDirectivesOutOfOrder()
        {
            var test = @"using System.Linq;
using System;

namespace ConsoleApplication1
{
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
                            new DiagnosticResultLocation("Test0.cs", line: 2, column: 1)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void ShouldAlphabetizeUsingDirectives_WhenDirectivesOutOfOrder()
        {
            var test = @"using System.Linq;
using System;

namespace ConsoleApplication1
{
    class TypeName
    {
    }
}";

            var fixtest = @"using System;
using System.Linq;

namespace ConsoleApplication1
{
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
