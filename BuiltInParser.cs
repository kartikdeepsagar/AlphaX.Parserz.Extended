using System;

namespace AlphaX.Parserz.Extended
{
    public static class BuiltInParser
    {
        private static IParser<DoubleResult> _numberParser;

        static BuiltInParser()
        {
            _numberParser = Parser.Number(false);
            CreateHumanTimeSpanParser();
        }

        #region Human TimeSpan Parser
        /// <summary>
        /// Gets the parser to parse human readable time span. <br></br>
        /// Time span format should be '[number]w [number]d [number]h [number]m' <br></br>
        /// For example: 1w 3d, 1d 4m, 12w 5h, 2w 3d 5h 1m<br></br>
        /// w - weeks<br></br>
        /// d - days<br></br>
        /// h - hours<br></br>
        /// m - minutes<br></br>
        /// </summary>
        public static IParser HumanTimeSpanParser { get; private set; }

        private static IParser<HumanTimeSpanParserResult> GetTimePartParser(char symbol, Action<HumanTimeSpan, double> valueSetter)
        {
            return _numberParser.AndThen(Parser.Char(symbol))
                .MapResult(x =>
                {
                    var span = new HumanTimeSpan();
                    valueSetter(span, (double)x.Value[0].Value);
                    var result = new HumanTimeSpanParserResult(span);
                    return result;
                });
        }

        private static void CreateHumanTimeSpanParser()
        {
            var wParser = GetTimePartParser('w', (span, value) => span.Weeks = value);
            var dParser = GetTimePartParser('d', (span, value) => span.Days = value);
            var hParser = GetTimePartParser('h', (span, value) => span.Hours = value);
            var mParser = GetTimePartParser('m', (span, value) => span.Minutes = value);

            var hourMinuteParser = hParser.Next(hourParserResult =>
            {
                return Parser.WhiteSpace.AndThen(mParser).MapResult(x => x.Value[1]).Many(0, 1)
                .MapResult(x =>
                {
                    if (x.Value.Length > 0)
                    {
                        var result = x.Value[0] as HumanTimeSpanParserResult;
                        result.Value.Hours = (hourParserResult as HumanTimeSpanParserResult).Value.Hours;
                        return result;
                    }
                    return hourParserResult;
                });
            });

            var dayHourMinuteParser = dParser.Next(dayParserResult =>
            {
                return Parser.WhiteSpace.AndThen(hourMinuteParser.Or(hParser).Or(mParser)).MapResult(x => x.Value[1]).Many(0, 1)
                .MapResult(x =>
                {
                    if (x.Value.Length > 0)
                    {
                        var result = x.Value[0] as HumanTimeSpanParserResult;
                        result.Value.Days = (dayParserResult as HumanTimeSpanParserResult).Value.Days;
                        return result;
                    }
                    return dayParserResult;
                });
            });

            var weekDayHourMinuteParser = wParser.Next(weekParserResult =>
            {
                return Parser.WhiteSpace.AndThen(dayHourMinuteParser.Or(hourMinuteParser).Or(dParser).Or(hParser).Or(mParser)).MapResult(x => x.Value[1]).Many(0, 1)
                .MapResult(x =>
                {
                    if (x.Value.Length > 0)
                    {
                        var result = x.Value[0] as HumanTimeSpanParserResult;
                        result.Value.Weeks = (weekParserResult as HumanTimeSpanParserResult).Value.Weeks;
                        return result;
                    }
                    return weekParserResult;
                });
            });

            HumanTimeSpanParser = mParser
                .Or(hourMinuteParser)
                .Or(dayHourMinuteParser)
                .Or(weekDayHourMinuteParser)
                .Or(wParser)
                .Or(dParser)
                .Or(hParser)
                .EndOfInput()
                .MapError(x => new ParserError(x.Index, $"Position ({x.Index}): Invalid timespan expression!"));
        }
        #endregion

        /// <summary>
        /// Gets a ranged number parser.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="canParseDecimal"></param>
        /// <returns></returns>
        public static IParser RangedNumberParser(double minimum, double maximum, bool canParseDecimal = true) => new RangedNumberParser(minimum, maximum, canParseDecimal);
    }
}
