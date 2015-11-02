using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<T> Create<T>(Func<TextPosition, IParseResult<T>> parse)
		{
			return new SimpleParser<T>(parse);
		}

		public static IParseResult<T> TryParse<T>(this IParser<T> parser, string text)
		{
			return parser.TryParse(text, 0);
		}

		public static IParseResult<T> TryParse<T>(this IParser<T> parser, string text, int startIndex)
		{
			return parser.TryParse(new TextPosition(text, startIndex));
		}

		public static T Parse<T>(this IParser<T> parser, string text)
		{
			return parser.Parse(text, 0);
		}

		public static T Parse<T>(this IParser<T> parser, string text, int startIndex)
		{
			return parser.TryParse(text, startIndex).Value;
		}

		private sealed class SimpleParser<T> : IParser<T>
		{
			public SimpleParser(Func<TextPosition, IParseResult<T>> parse)
			{
				m_parse = parse;
			}

			public IParseResult<T> TryParse(TextPosition position)
			{
				return m_parse(position);
			}

			readonly Func<TextPosition, IParseResult<T>> m_parse;
		}
	}
}
