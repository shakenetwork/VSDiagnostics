﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.CompareBooleanToTrueLiteral;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class CompareBooleanToTrueLiteralTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new CompareBooleanToTrueLiteralAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new CompareBooleanToTrueLiteralCodeFix();

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_EqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = true;
            if (isAwesome == true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = true;
            if (isAwesome)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparisonAsReturnValue_EqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = true;
            return isAwesome == true;
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = true;
            return isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithComplicatedTrueLiteralComparison_EqualsOperator_FirstComparisonIsEquals_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name == ""Jeroen"" == true).ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name == ""Jeroen"").ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithComplicatedTrueLiteralComparison_EqualsOperator_FirstComparisonIsNotEquals_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name != ""Jeroen"" == true).ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name != ""Jeroen"").ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithUnrelatedTrueLiteral_EqualsOperator_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = true;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimplifiedExpression_EqualsOperator_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = true;
            return isAwesome;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_ComparedToBooleanAsString_EqualsOperator_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            if (""someString"" == ""true"")
            {

            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithTrueLiteralAsLefthandValue_EqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return true == isAwesome;
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = true;
            if (isAwesome != true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool isAwesome = true;
            if (!isAwesome)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparisonAsReturnValue_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = true;
            return isAwesome != true;
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = true;
            return !isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithComplicatedTrueLiteralComparison_NotEqualsOperator_FirstComparisonIsEquals_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name == ""Jeroen"" != true).ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name != ""Jeroen"").ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithComplicatedTrueLiteralComparison_NotEqualsOperator_FirstComparisonIsNotEquals_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name != ""Jeroen"" != true).ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        Student[] Method()
        {
            var students = new List<Student>().Where(x => x.Name == ""Jeroen"").ToArray();
            return students;
        }
    }

    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimplifiedExpression_NotEqualsOperator_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = true;
            return !isAwesome;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_ComparedToBooleanAsString_NotEqualsOperator_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            if (""someString"" != ""true"")
            {

            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithTrueLiteralAsLefthandValue_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return true != isAwesome;
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method()
        {
            bool isAwesome = false;
            return !isAwesome;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithNullableBool_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            bool? b = null;
            if(b == true)
            {

            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithNullableBool_AsMethodInvocation_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if(GetBoolean() == true)
            {

            }
        }

        bool? GetBoolean()
        {
            return false;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_AsMethodInvocation_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (GetBoolean() == true)
            {

            }
        }

        bool GetBoolean()
        {
            return false;
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (GetBoolean())
            {

            }
        }

        bool GetBoolean()
        {
            return false;
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_GreaterThanOperator_EqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome > 1 == true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome > 1)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_LessThanOperator_EqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome < 1 == true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome < 1)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_GreaterThanOrEqualsOperator_EqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome >= 1 == true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome >= 1)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_LessThanOrEqualsOperator_EqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome <= 1 == true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome <= 1)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_GreaterThanOperator_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome > 1 != true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome <= 1)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_LessThanOperator_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome < 1 != true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome >= 1)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_GreaterThanOrEqualsOperator_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome >= 1 != true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome < 1)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void CompareBooleanToTrueLiteral_WithSimpleTrueLiteralComparison_LessThanOrEqualsOperator_NotEqualsOperator_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome <= 1 != true)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int isAwesome = 0;
            if (isAwesome > 1)
            {
                Console.WriteLine(""awesome"");
            }
        }
    }
}";

            VerifyDiagnostic(original, CompareBooleanToTrueLiteralAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}