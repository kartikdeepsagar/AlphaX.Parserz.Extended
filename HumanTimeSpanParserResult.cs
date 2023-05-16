namespace AlphaX.Parserz.Extended
{
    public class HumanTimeSpanParserResult : ParserResult<HumanTimeSpan>
    {
        public static ParserResultType HumanTimeSpan = new ParserResultType("HumanTimeSpan");

        public HumanTimeSpanParserResult(HumanTimeSpan value) : base(value, HumanTimeSpan)
        {

        }
    }
}
