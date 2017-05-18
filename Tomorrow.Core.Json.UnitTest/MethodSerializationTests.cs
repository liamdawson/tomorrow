using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomorrow.Core.Json.Serialization;

namespace Tomorrow.Core.Json.UnitTest
{
    [TestClass]
    public class MethodSerializationTests
    {
        private const string TestString = "test string";

        private class TestClass
        {
            public void Nop() { }
            public string EchoString(string str) => str;

            public void OverloadedFunction(object shouldNotBeInvoked) => throw new InvalidOperationException();
            public void OverloadedFunction() { }
            public void OverloadedFunction(string shouldBeInvoked) { }

            public T ReturnGeneric<T>(T input) => input;

            public static string EchoStringStatic(string str) => str;
        }

        [TestMethod]
        public void SimplestInstanceMethodSurvivesRoundTrip()
        {
            var methodInfo = ((Action)new TestClass().Nop).GetMethodInfo();

            Assert.AreEqual(methodInfo, ((SimpleMethodPointer)methodInfo).MethodInfo);
        }

        [TestMethod]
        public void SimpleParameterInstanceMethodCanBeInvoked()
        {
            var methodInfo = ((Func<string, string>)new TestClass().EchoString).GetMethodInfo();

            var postRepresentation = ((SimpleMethodPointer) methodInfo).MethodInfo;

            Assert.AreEqual(methodInfo, postRepresentation);

            Assert.AreEqual(TestString, postRepresentation.Invoke(new TestClass(), new object[] {TestString}));
        }

        [TestMethod]
        public void GenericParameterInstanceMethodCanBeInvoked()
        {
            var methodInfo = ((Func<string, string>)new TestClass().ReturnGeneric<string>).GetMethodInfo();

            var postRepresentation = ((SimpleMethodPointer) methodInfo).MethodInfo;

            Assert.AreEqual(methodInfo, postRepresentation);

            Assert.AreEqual(TestString, postRepresentation.Invoke(new TestClass(), new object[] {TestString}));
        }

        [TestMethod]
        public void SimpleParameterStaticMethodCanBeInvoked()
        {
            var methodInfo = ((Func<string, string>)TestClass.EchoStringStatic).GetMethodInfo();

            var postRepresentation = ((SimpleMethodPointer) methodInfo).MethodInfo;

            Assert.AreEqual(methodInfo, postRepresentation);

            Assert.AreEqual(TestString, postRepresentation.Invoke(null, new object[] {TestString}));
        }

        [TestMethod]
        public void CorrectlyPicksOverloadsWithDifferingArity()
        {
            // get method only taking one argument
            var methodInfo = ((Action)new TestClass().OverloadedFunction).GetMethodInfo();

            var postRepresentation = ((SimpleMethodPointer) methodInfo).MethodInfo;

            Assert.AreEqual(methodInfo, postRepresentation);
            postRepresentation.Invoke(new TestClass(), new object[] { });
        }

        [TestMethod]
        public void CorrectlyPicksOverloadsWithDifferingParameterTypes()
        {
            // get method only taking one argument
            var methodInfo = ((Action<string>)new TestClass().OverloadedFunction).GetMethodInfo();

            var postRepresentation = ((SimpleMethodPointer) methodInfo).MethodInfo;

            Assert.AreEqual(methodInfo, postRepresentation);
            // will throw if object version is chosen
            postRepresentation.Invoke(new TestClass(), new object[] { TestString });
        }
    }
}
