using NUnit.Framework;

namespace AlphaX.Parserz.Extended.Tests
{
    public class HumanTimeSpanParserTests
    {
        [TestCase("1w 3d 4h 3m", 1, 3, 4, 3)]
        [TestCase("10w 4d", 10, 4, null, null)]
        [TestCase("1w 4m", 1, null, null, 4)]
        [TestCase("4d 1m", null, 4, null, 1)]
        [TestCase("1d 2h", null, 1, 2, null)]
        [TestCase("12w", 12, null, null, null)]
        [TestCase("2w 1d 2h 5m", 2, 1, 2, 5)]
        public void HumanTimeSpanParserTests_Success_Test(string value, double? weeks, double? days, double? hours, double? minutes)
        {
            var resultState = BuiltInParser.HumanTimeSpanParser.Run(value);
            Assert.AreEqual(resultState.Result.Type, HumanTimeSpanParserResult.HumanTimeSpan);
            Assert.IsFalse(resultState.IsError);
            Assert.IsInstanceOf(typeof(HumanTimeSpanParserResult), resultState.Result);

            var result = resultState.Result as HumanTimeSpanParserResult;
            Assert.AreEqual(result.Value.Weeks, weeks);
            Assert.AreEqual(result.Value.Days, days);
            Assert.AreEqual(result.Value.Hours, hours);
            Assert.AreEqual(result.Value.Minutes, minutes);
        }

        [TestCase("4m 1w")]
        [TestCase("5m 1h")]
        [TestCase("4d1w")]
        [TestCase("wd")]
        [TestCase("w0m")]
        [TestCase("1d 1m 1w")]
        public void HumanTimeSpanParserTests_Failure_Test(string value)
        {
            var resultState = BuiltInParser.HumanTimeSpanParser.Run(value);
            Assert.IsTrue(resultState.IsError);
        }
    }
}