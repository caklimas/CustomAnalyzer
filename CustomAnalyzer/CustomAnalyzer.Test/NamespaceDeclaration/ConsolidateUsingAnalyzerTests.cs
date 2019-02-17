using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using CustomAnalyzer.NamespaceDeclaration;

namespace CustomAnalyzer.Test.NamespaceDeclaration
{
    [TestClass]
    public class ConsolidateUsingAnalyzerTests : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void ReturnsWarning_WhenDirectiveHasExtraTrivia()
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
                Id = ConsolidateUsingAnalyzer.DiagnosticId,
                Message = ConsolidateUsingAnalyzer.MessageFormat.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line: 5, column: 5)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void ShouldConsolidateUsingDirectives_WhenDirectiveHasExtraTrivia()
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

        [TestMethod]
        public void ShouldConsolidateUsingDirectives_WhenDirectivesHaveExtraTrivia()
        {
            var test = @"namespace ConsoleApplication1
{
    using System;

    using System.Linq;

    using System.Text;

    class TypeName
    {
    }
}";

            var fixtest = @"namespace ConsoleApplication1
{
    using System;
    using System.Linq;
    using System.Text;

    class TypeName
    {
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ConsolidateUsingAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConsolidateUsingAnalyzer();
        }
    }
}