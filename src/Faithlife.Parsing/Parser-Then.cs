using System;
using System.Collections.Generic;
using System.Linq;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Executes one parser after another.
		/// </summary>
		public static IParser<U> Then<T, U>(this IParser<T> parser, Func<T, IParser<U>> convertValueToNextParser)
		{
			return Create(position => parser.TryParse(position).MapSuccess(result => convertValueToNextParser(result.Value).TryParse(result.NextPosition)));
		}

		/// <summary>
		/// Converts any successfully parsed value.
		/// </summary>
		public static IParser<U> Select<T, U>(this IParser<T> parser, Func<T, U> convertValue)
		{
			return parser.Then(value => Success(convertValue(value)));
		}

		/// <summary>
		/// Succeeds with the specified value if the parser is successful.
		/// </summary>
		public static IParser<U> Success<T, U>(this IParser<T> parser, U value)
		{
			return parser.Select(_ => value);
		}

		/// <summary>
		/// Concatenates the two successfully parsed collections.
		/// </summary>
		public static IParser<IReadOnlyList<T>> Concat<T>(this IParser<IEnumerable<T>> firstParser, IParser<IEnumerable<T>> secondParser)
		{
			return firstParser.Then(firstValue => secondParser.Select(x => firstValue.Concat(x).ToList()));
		}

		/// <summary>
		/// Appends a successfully parsed value to the end of a successfully parsed collection.
		/// </summary>
		public static IParser<IReadOnlyList<T>> Append<T>(this IParser<IEnumerable<T>> firstParser, IParser<T> secondParser)
		{
			return firstParser.Concat(secondParser.Once());
		}

		/// <summary>
		/// Used to support LINQ query syntax.
		/// </summary>
		public static IParser<V> SelectMany<T, U, V>(this IParser<T> parser, Func<T, IParser<U>> selector, Func<T, U, V> projector)
		{
			return parser.Then(t => selector(t).Select(u => projector(t, u)));
		}
	}
}
