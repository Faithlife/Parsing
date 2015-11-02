using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Succeeds if the parser succeeds. The value is a one-item collection of the successfully parsed item.
		/// </summary>
		public static IParser<IReadOnlyList<T>> Once<T>(this IParser<T> parser)
		{
			return parser.Repeat(1);
		}

		/// <summary>
		/// Always succeeds. The value is a one-item collection of a single successfully parsed item; otherwise an empty collection.
		/// </summary>
		public static IParser<IReadOnlyList<T>> AtMostOnce<T>(this IParser<T> parser)
		{
			return parser.AtMost(1);
		}

		/// <summary>
		/// Always succeeds. The value is a collection of at most the specified number of successfully parsed items.
		/// </summary>
		public static IParser<IReadOnlyList<T>> AtMost<T>(this IParser<T> parser, int atMost)
		{
			return parser.DoRepeat(0, atMost);
		}

		/// <summary>
		/// Always succeeds. The value is a collection of as many items as can be successfully parsed.
		/// </summary>
		public static IParser<IReadOnlyList<T>> Many<T>(this IParser<T> parser)
		{
			return parser.AtLeast(0);
		}

		/// <summary>
		/// Succeeds if the parser succeeds at least once. The value is a collection of as many items as can be successfully parsed.
		/// </summary>
		public static IParser<IReadOnlyList<T>> AtLeastOnce<T>(this IParser<T> parser)
		{
			return parser.AtLeast(1);
		}

		/// <summary>
		/// Succeeds if the parser succeeds at least the specified number of times. The value is a collection of as many items as can be successfully parsed.
		/// </summary>
		public static IParser<IReadOnlyList<T>> AtLeast<T>(this IParser<T> parser, int atLeast)
		{
			return parser.DoRepeat(atLeast, null);
		}

		/// <summary>
		/// Succeeds if the parser succeeds the specified number of times. The value is a collection of the parsed items.
		/// </summary>
		public static IParser<IReadOnlyList<T>> Repeat<T>(this IParser<T> parser, int exactly)
		{
			return parser.DoRepeat(exactly, exactly);
		}

		/// <summary>
		/// Succeeds if the specified parser succeeds at least once, requiring and ignoring the specified delimiter between each item.
		/// </summary>
		public static IParser<IReadOnlyList<T>> Delimited<T, U>(this IParser<T> parser, IParser<U> delimiter)
		{
			return
				from first in parser.Once()
				from rest in parser.PrecededBy(delimiter).Many()
				select first.Concat(rest).ToList();
		}

		/// <summary>
		/// Succeeds if the specified parser succeeds at least once, requiring and ignoring the specified delimiter between each item,
		/// and allowing a single optional trailing delimiter.
		/// </summary>
		public static IParser<IReadOnlyList<T>> DelimitedAllowTrailing<T, U>(this IParser<T> parser, IParser<U> delimiter)
		{
			return
				from first in parser.Once()
				from rest in parser.PrecededBy(delimiter).Many()
				from trailing in delimiter.OrDefault()
				select first.Concat(rest).ToList();
		}

		private static IParser<IReadOnlyList<T>> DoRepeat<T>(this IParser<T> parser, int atLeast, int? atMost)
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
