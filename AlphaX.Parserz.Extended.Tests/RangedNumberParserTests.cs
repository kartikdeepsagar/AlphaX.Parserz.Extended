using System;
using AlphaX.Parserz.Extended;
using NUnit.Framework;

namespace AlphaX.Parserz.Extended.Tests
{
    public class RangedNumberParserTests
    {
        [TestCase("2", 1, 10)]
        [TestCase("7", 5, 10)]
        [TestCase("10", 10, 10)]
        [TestCase("15", 10, 20)]
        [TestCase("11", 1, 20)]
        [TestCase("2.42", 1.5, 2.5)]
        public void RangedNumberParser_Success_Test(string value, double min, double max)
        {
            var resultState = BuiltInParser.RangedNumberParser(min, max).Run(value);
            Assert.AreEqual(resultState.Result.Type, ParserResultType.Number);
            Assert.IsFalse(resultState.IsError);
            Assert.IsInstanceOf(typeof(DoubleResult), resultState.Result);
        }

        [TestCase("232", 1, 10)]
        [TestCase("12", 5, 10)]
        [TestCase("231", 10, 10)]
        [TestCase("23", 10, 20)]
        [TestCase("099", 1, 20)]
        [TestCase("2.42", 2.5, 3.0)]
        public void RangedNumberParser_Failure_Test(string value, double min, double max)
        {
            var resultState = BuiltInParser.RangedNumberParser(min, max).Run(value);
            Assert.IsTrue(resultState.IsError);
        }

        [TestCase("232", 1000, 10)]
        [TestCase("232", 1, 0)]
        public void RangedNumberParser_Exception_Test(string value, double min, double max)
        {
            Assert.Throws<InvalidOperationException>(() => BuiltInParser.RangedNumberParser(min, max).Run(value));
        }
    }
}