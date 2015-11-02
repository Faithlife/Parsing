using System.Collections.Generic;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<IReadOnlyList<T>> Once<T>(this IParser<T> parser)
		{
			return parser.Repeat(1);
		}

		public static IParser<IReadOnlyList<T>> AtMostOnce<T>(this IParser<T> parser)
		{
			return parser.AtMost(1);
		}

		public static IParser<IReadOnlyList<T>> AtMost<T>(this IParser<T> parser, int atMost)
		{
			return parser.Repeat(0, atMost);
		}

		public static IParser<IReadOnlyList<T>> Many<T>(this IParser<T> parser)
		{
			return parser.AtLeast(0);
		}

		public static IParser<IReadOnlyList<T>> AtLeastOnce<T>(this IParser<T> parser)
		{
			return parser.AtLeast(1);
		}

		public static IParser<IReadOnlyList<T>> AtLeast<T>(this IParser<T> parser, int atLeast)
		{
			return parser.Repeat(atLeast, null);
		}

		public static IParser<IReadOnlyList<T>> Repeat<T>(this IParser<T> parser, int exactly)
		{
			return parser.Repeat(exactly, exactly);
		}

		public static IParser<IReadOnlyList<T>> Repeat<T>(this IParser<T> parser, int atLeast, int? atMost)
		{
			return Create(position =>
			{
				List<T> values = new List<T>(capacity: atLeast);
				TextPosition remainder = position;
				int repeated = 0;

				while (true)
				{
					if (repeated >= atMost)
						break;
					IParseResult<T> result = parser.TryParse(remainder);
					if (!result.Success || result.NextPosition == remainder)
						break;
					values.Add(result.Value);
					remainder = result.NextPosition;
					repeated++;
				}

				return repeated >= atLeast ? ParseResult.Success<IReadOnlyList<T>>(values, remainder) : ParseResult.Failure<IReadOnlyList<T>>(position);
			});
		}
	}
}
