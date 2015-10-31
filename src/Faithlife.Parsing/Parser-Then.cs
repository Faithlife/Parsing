using System;
using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<U> Then<T, U>(this IParser<T> parser, Func<T, IParser<U>> convertValueToNextParser)
		{
			return Parser.Create(input => parser.TryParse(input).IfSuccess(result => convertValueToNextParser(result.Value).TryParse(result.Remainder)));
		}

		public static IParser<U> Then<T, U>(this IParser<T> parser, Func<IParser<U>> createNextParser)
		{
			return parser.Then(_ => createNextParser());
		}

		public static IParser<U> Then<T, U>(this IParser<T> parser, IParser<U> nextParser)
		{
			return parser.Then(_ => nextParser);
		}

		public static IParser<U> Select<T, U>(this IParser<T> parser, Func<T, U> convertValue)
		{
			return parser.Then(value => Parser.Return(convertValue(value)));
		}

		public static IParser<U> Return<T, U>(this IParser<T> parser, Func<U> createValue)
		{
			return parser.Select(_ => createValue());
		}

		public static IParser<U> Return<T, U>(this IParser<T> parser, U value)
		{
			return parser.Select(_ => value);
		}

		public static IParser<IReadOnlyList<T>> Concat<T>(this IParser<IEnumerable<T>> firstParser, IParser<IEnumerable<T>> secondParser)
		{
			return firstParser.Then(firstValue => secondParser.Select(x => firstValue.Concat(x).ToList()));
		}

		public static IParser<IReadOnlyList<T>> Append<T>(this IParser<IEnumerable<T>> firstParser, IParser<T> secondParser)
		{
			return firstParser.Concat(secondParser.Once());
		}

		public static IParser<V> SelectMany<T, U, V>(this IParser<T> parser, Func<T, IParser<U>> selector, Func<T, U, V> projector)
		{
			return parser.Then(t => selector(t).Select(u => projector(t, u)));
		}
	}
}
