using System;
using AlphaX.Parserz.Extended.Resources;

namespace AlphaX.Parserz.Extended
{
    internal class RangedNumberParser : Parser<DoubleResult>
    {
        private readonly IParser<DoubleResult> _numberParser;
        public double Minimum { get; }
        public double Maximum { get; }

        public RangedNumberParser(double minimum, double maximum, bool canParseDecimal)
        {
            _numberParser = Parser.Number(canParseDecimal);

            if (minimum > maximum)
                throw new InvalidOperationException("Minimum value shouldn't be greated than maximum value.");

            Minimum = minimum;
            Maximum = maximum;
        }

        protected override IParserState ParseInput(IParserState inputState)
        {
            var state = _numberParser.Parse(inputState);

            if (!state.IsError)
            {
                var value = (double)state.Result.Value;

                if (value < Minimum || value > Maximum)
                    return ParserStates.Error(inputState, new ParserError(inputState.Index,
                   string.Format(ParserMessages.UnexpectedInputError, inputState.Index, $"number from {Minimum} to {Maximum}", value)));
            }

            return state;
        }
    }
}
