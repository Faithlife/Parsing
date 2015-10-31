using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<T> PrecededBy<T, U>(this IParser<T> parser, IParser<U> precededBy)
		{
			return precededBy.Then(parser);
		}

		public static IParser<T> FollowedBy<T, U>(this IParser<T> parser, IParser<U> followedBy)
		{
			return parser.Then(followedBy.Return);
		}

		public static IParser<T> Bracketed<T, U, V>(this IParser<T> parser, IParser<U> precededBy, IParser<V> followedBy)
		{
			return parser.PrecededBy(precededBy).FollowedBy(followedBy);
		}

		public static IParser<T> TrimStart<T>(this IParser<T> parser)
		{
			return parser.TrimStart(Parser.WhiteSpace);
		}

		public static IParser<T> TrimStart<T, U>(this IParser<T> parser, IParser<U> trimParser)
		{
			return parser.PrecededBy(trimParser.Many());
		}

		public static IParser<T> TrimEnd<T>(this IParser<T> parser)
		{
			return parser.TrimEnd(Parser.WhiteSpace);
		}

		public static IParser<T> TrimEnd<T, U>(this IParser<T> parser, IParser<U> trimParser)
		{
			return parser.FollowedBy(trimParser.Many());
		}

		public static IParser<T> Trim<T>(this IParser<T> parser)
		{
			return parser.Trim(Parser.WhiteSpace);
		}

		public static IParser<T> Trim<T, U>(this IParser<T> parser, IParser<U> trimParser)
		{
			return parser.TrimStart(trimParser).TrimEnd(trimParser);
		}

		public static IParser<IReadOnlyList<T>> Delimited<T, U>(this IParser<T> parser, IParser<U> delimiter)
		{
			return from first in parser.Once()
				   from rest in parser.PrecededBy(delimiter).Many()
				   select first.Concat(rest).ToList();
		}

		public static IParser<IReadOnlyList<T>> DelimitedAllowTrailing<T, U>(this IParser<T> parser, IParser<U> delimiter)
		{
			return from first in parser.Once()
				   from rest in parser.PrecededBy(delimiter).Many()
				   from trailing in delimiter.OrDefault()
				   select first.Concat(rest).ToList();
		}
	}
}
