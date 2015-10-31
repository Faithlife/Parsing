using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<T> Create<T>(Func<Input, IResult<T>> parse)
		{
			return new SimpleParser<T>(parse);
		}

		public static IParser<T> Return<T>(T value)
		{
			return Parser.Create(input => Result.Success(value, input));
		}

		public static IResult<T> TryParse<T>(this IParser<T> parser, string text)
		{
			return parser.TryParse(text, 0);
		}

		public static IResult<T> TryParse<T>(this IParser<T> parser, string text, int startIndex)
		{
			return parser.TryParse(new Input(new Source(text), startIndex));
		}

		public static T Parse<T>(this IParser<T> parser, string text)
		{
			return parser.Parse(text, 0);
		}

		public static T Parse<T>(this IParser<T> parser, string text, int startIndex)
		{
			IResult<T> result = parser.TryParse(text, startIndex);
			if (!result.Success)
				throw new ParseException(result);
			return result.Value;
		}

		public static IParser<T> End<T>(this IParser<T> parser)
		{
			return Parser.Create(input => parser.TryParse(input).IfSuccess(result => result.Remainder.AtEnd ? result : Result.Failure<T>(result.Remainder)));
		}

		private sealed class SimpleParser<T> : IParser<T>
		{
			public SimpleParser(Func<Input, IResult<T>> parse)
			{
				m_parse = parse;
			}

			public IResult<T> TryParse(Input input)
			{
				return m_parse(input);
			}

			readonly Func<Input, IResult<T>> m_parse;
		}
	}
}
