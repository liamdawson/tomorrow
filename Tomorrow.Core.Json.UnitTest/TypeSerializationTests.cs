using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomorrow.Core.Json.Serialization;

namespace Tomorrow.Core.Json.UnitTest
{
    [TestClass]
    public class TypeSerializationTests
    {
        [TestMethod]
        public void CoreTypeSurvivesRoundTrip()
        {
            var type = typeof(string);

            Assert.AreEqual(type, ((SimpleTypePointer)type).Type);
        }

        private class TestClass { }

        [TestMethod]
        public void PrivateTypeSurvivesRoundTrip()
        {
            var type = typeof(TestClass);

            Assert.AreEqual(type, ((SimpleTypePointer)type).Type);
        }

        // ReSharper disable once UnusedTypeParameter
        private class GenericTestClass<T> { }

        [TestMethod]
        public void GenericTypeSurvivesRoundTrip()
        {
            var type = typeof(GenericTestClass<string>);

            Assert.AreEqual(type, ((SimpleTypePointer)type).Type);
        }
    }
}
