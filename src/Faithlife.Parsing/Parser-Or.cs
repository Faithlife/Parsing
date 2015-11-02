using System.Collections.Generic;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<T> Or<T>(IEnumerable<IParser<T>> parsers)
		{
			return Create(position =>
			{
				IParseResult<T> firstEmptySuccess = null;
				IParseResult<T> firstFailure = null;

				foreach (IParser<T> parser in parsers)
				{
					IParseResult<T> result = parser.TryParse(position);
					if (!result.Success)
						firstFailure = firstFailure ?? result;
					else if (result.NextPosition == position)
						firstEmptySuccess = firstEmptySuccess ?? result;
					else
						return result;
				}

				return firstEmptySuccess ?? firstFailure ?? ParseResult.Failure<T>(position);
			});
		}

		public static IParser<T> Or<T>(params IParser<T>[] parsers)
		{
			return Or((IEnumerable<IParser<T>>) parsers);
		}

		public static IParser<T> Or<T>(this IParser<T> first, IParser<T> second)
		{
			return Or(new[] { first, second });
		}

		public static IParser<T> OrDefault<T>(this IParser<T> parser)
		{
			return parser.OrDefault(default(T));
		}

		public static IParser<T> OrDefault<T>(this IParser<T> parser, T value)
		{
			return parser.Or(Success(value));
		}

		public static IParser<IEnumerable<T>> OrEmpty<T>(this IParser<IEnumerable<T>> parser)
		{
			return parser.OrDefault(new T[0]);
		}

		public static IParser<IReadOnlyCollection<T>> OrEmpty<T>(this IParser<IReadOnlyCollection<T>> parser)
		{
			return parser.OrDefault(new T[0]);
		}

		public static IParser<IReadOnlyList<T>> OrEmpty<T>(this IParser<IReadOnlyList<T>> parser)
		{
			return parser.OrDefault(new T[0]);
		}
	}
}
