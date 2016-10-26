using System.Linq;
using System.Threading;
using Microsoft.Vbe.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rubberduck.Inspections;
using Rubberduck.Parsing;
using Rubberduck.Parsing.VBA;
using Rubberduck.VBEditor.Application;
using RubberduckTests.Mocks;
using CodeModule = Rubberduck.VBEditor.SafeComWrappers.VBA.CodeModule;

namespace RubberduckTests.Inspections
{
    [TestClass]
    public class VariableTypeNotDeclaredInspectionTests
    {
        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_ReturnsResult_Parameter()
        {
            const string inputCode =
@"Sub Foo(arg1)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_ReturnsResult_MultipleParams()
        {
            const string inputCode =
@"Sub Foo(arg1, arg2)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.AreEqual(2, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_DoesNotReturnResult_Parameter()
        {
            const string inputCode =
@"Sub Foo(arg1 As Date)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.AreEqual(0, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_ReturnsResult_SomeTypesNotDeclared_Parameters()
        {
            const string inputCode =
@"Sub Foo(arg1, arg2 As String)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_ReturnsResult_SomeTypesNotDeclared_Variables()
        {
            const string inputCode =
@"Sub Foo()
    Dim var1
    Dim var2 As Date
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_ReturnsResult_Variable()
        {
            const string inputCode =
@"Sub Foo()
    Dim var1
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.AreEqual(1, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_ReturnsResult_MultipleVariables()
        {
            const string inputCode =
@"Sub Foo()
    Dim var1
    Dim var2
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.AreEqual(2, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_DoesNotReturnResult_Variable()
        {
            const string inputCode =
@"Sub Foo()
    Dim var1 As Integer
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.AreEqual(0, inspectionResults.Count());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_Ignored_DoesNotReturnResult()
        {
            const string inputCode =
@"'@Ignore VariableTypeNotDeclared
Sub Foo(arg1)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            var inspectionResults = inspection.GetInspectionResults();

            Assert.IsFalse(inspectionResults.Any());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_QuickFixWorks_Parameter()
        {
            const string inputCode =
@"Sub Foo(arg1)
End Sub";

            const string expectedCode =
@"Sub Foo(arg1 As Variant)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var project = vbe.Object.VBProjects.Item(0);
            var module = project.VBComponents.Item(0).CodeModule;
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            inspection.GetInspectionResults().First().QuickFixes.First().Fix();

            var actual = new CodeModule(module).Content();
            Assert.AreEqual(expectedCode, actual);
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_QuickFixWorks_SubNameContainsParameterName()
        {
            const string inputCode =
@"Sub Foo(Foo)
End Sub";

            const string expectedCode =
@"Sub Foo(Foo As Variant)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var project = vbe.Object.VBProjects.Item(0);
            var module = project.VBComponents.Item(0).CodeModule;
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            inspection.GetInspectionResults().First().QuickFixes.First().Fix();

            var actual = new CodeModule(module).Content();
            Assert.AreEqual(expectedCode, actual);
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_QuickFixWorks_Variable()
        {
            const string inputCode =
@"Sub Foo()
    Dim var1
End Sub";

            const string expectedCode =
@"Sub Foo()
    Dim var1 As Variant
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var project = vbe.Object.VBProjects.Item(0);
            var module = project.VBComponents.Item(0).CodeModule;
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            inspection.GetInspectionResults().First().QuickFixes.First().Fix();

            Assert.AreEqual(expectedCode, new CodeModule(module).Content());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_QuickFixWorks_ParameterWithoutDefaultValue()
        {
            const string inputCode =
@"Sub Foo(ByVal Fizz)
End Sub";

            const string expectedCode =
@"Sub Foo(ByVal Fizz As Variant)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var project = vbe.Object.VBProjects.Item(0);
            var module = project.VBComponents.Item(0).CodeModule;
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            inspection.GetInspectionResults().First().QuickFixes.First().Fix();

            Assert.AreEqual(expectedCode, new CodeModule(module).Content());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_QuickFixWorks_ParameterWithDefaultValue()
        {
            const string inputCode =
@"Sub Foo(ByVal Fizz = False)
End Sub";

            const string expectedCode =
@"Sub Foo(ByVal Fizz As Variant = False)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var project = vbe.Object.VBProjects.Item(0);
            var module = project.VBComponents.Item(0).CodeModule;
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            inspection.GetInspectionResults().First().QuickFixes.First().Fix();

            Assert.AreEqual(expectedCode, new CodeModule(module).Content());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void VariableTypeNotDeclared_IgnoreQuickFixWorks()
        {
            const string inputCode =
@"Sub Foo(arg1)
End Sub";

            const string expectedCode =
@"'@Ignore VariableTypeNotDeclared
Sub Foo(arg1)
End Sub";

            //Arrange
            var builder = new MockVbeBuilder();
            VBComponent component;
            var vbe = builder.BuildFromSingleStandardModule(inputCode, out component);
            var project = vbe.Object.VBProjects.Item(0);
            var module = project.VBComponents.Item(0).CodeModule;
            var mockHost = new Mock<IHostApplication>();
            mockHost.SetupAllProperties();
            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(new Mock<ISinks>().Object));

            parser.Parse(new CancellationTokenSource());
            if (parser.State.Status >= ParserState.Error) { Assert.Inconclusive("Parser Error"); }

            var inspection = new VariableTypeNotDeclaredInspection(parser.State);
            inspection.GetInspectionResults().First().QuickFixes.Single(s => s is IgnoreOnceQuickFix).Fix();

            Assert.AreEqual(expectedCode, new CodeModule(module).Content());
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void InspectionType()
        {
            var inspection = new VariableTypeNotDeclaredInspection(null);
            Assert.AreEqual(CodeInspectionType.LanguageOpportunities, inspection.InspectionType);
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void InspectionName()
        {
            const string inspectionName = "VariableTypeNotDeclaredInspection";
            var inspection = new VariableTypeNotDeclaredInspection(null);

            Assert.AreEqual(inspectionName, inspection.Name);
        }
    }
}
