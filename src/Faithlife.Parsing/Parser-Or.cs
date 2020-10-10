using System;
using System.Collections.Generic;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Succeeds with a successful parser, if any.
		/// </summary>
		/// <remarks>The first successful parser that advances the text position is returned.
		/// Otherwise, the first successful parser that does not advance the text position is returned.
		/// Otherwise, the first failure is returned.</remarks>
		public static IParser<T> Or<T>(IEnumerable<IParser<T>> parsers)
		{
			return Create(position =>
			{
				IParseResult<T>? firstEmptySuccess = null;
				IParseResult<T>? firstFailure = null;

				foreach (var parser in parsers)
				{
					var result = parser.TryParse(position);
					if (!result.Success)
						firstFailure ??= result;
					else if (result.NextPosition == position)
						firstEmptySuccess ??= result;
					else
						return result;
				}

				return firstEmptySuccess ?? firstFailure ?? ParseResult.Failure<T>(position);
			});
		}

		/// <summary>
		/// Succeeds with a successful parser, if any.
		/// </summary>
		/// <remarks>The first successful parser that advances the text position is returned.
		/// Otherwise, the first successful parser that does not advance the text position is returned.
		/// Otherwise, the first failure is returned.</remarks>
		public static IParser<T> Or<T>(params IParser<T>[] parsers) => Or((IEnumerable<IParser<T>>) parsers);

		/// <summary>
		/// Succeeds with a successful parser, if any.
		/// </summary>
		/// <remarks>The first successful parser that advances the text position is returned.
		/// Otherwise, the first successful parser that does not advance the text position is returned.
		/// Otherwise, the first failure is returned.</remarks>
		public static IParser<T> Or<T>(this IParser<T> first, IParser<T> second) => Or(new[] { first, second });

		/// <summary>
		/// Succeeds with the default value if the parser fails.
		/// </summary>
		public static IParser<T> OrDefault<T>(this IParser<T> parser) => parser.OrDefault(default!);

		/// <summary>
		/// Succeeds with the default value if the parser fails.
		/// </summary>
		public static IParser<T> OrDefault<T>(this IParser<T> parser, T value) => parser.Or(Success(value));

		/// <summary>
		/// Succeeds with an empty collection if the parser fails.
		/// </summary>
		public static IParser<IEnumerable<T>> OrEmpty<T>(this IParser<IEnumerable<T>> parser) => parser.OrDefault(Array.Empty<T>());

		/// <summary>
		/// Succeeds with an empty collection if the parser fails.
		/// </summary>
		public static IParser<IReadOnlyCollection<T>> OrEmpty<T>(this IParser<IReadOnlyCollection<T>> parser) => parser.OrDefault(Array.Empty<T>());

		/// <summary>
		/// Succeeds with an empty collection if the parser fails.
		/// </summary>
		public static IParser<IReadOnlyList<T>> OrEmpty<T>(this IParser<IReadOnlyList<T>> parser) => parser.OrDefault(Array.Empty<T>());
	}
}
