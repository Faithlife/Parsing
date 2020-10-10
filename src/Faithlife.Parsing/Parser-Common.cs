using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Succeeds only at the end of the text.
		/// </summary>
		public static IParser<T> End<T>(this IParser<T> parser)
		{
			return Create(position => parser.TryParse(position)
				.MapSuccess(result => result.NextPosition.IsAtEnd() ? result : ParseResult.Failure<T>(result.NextPosition)));
		}

		/// <summary>
		/// Reports a named failure with the specified name if the parser fails.
		/// </summary>
		public static IParser<T> Named<T>(this IParser<T> parser, string name)
		{
			return Create(position =>
			{
				IParseResult<T> result = parser.TryParse(position);
				if (!result.Success)
					result.NextPosition.ReportNamedFailure(name, result);
				return result;
			});
		}

		/// <summary>
		/// Wraps the text position and length around a successfully parsed value.
		/// </summary>
		public static IParser<Positioned<T>> Positioned<T>(this IParser<T> parser)
		{
			return Create(position => parser.TryParse(position)
				.MapSuccess(result => ParseResult.Success(new Positioned<T>(result.Value, position, result.NextPosition.Index - position.Index), result.NextPosition)));
		}

		/// <summary>
		/// Refers to another parser indirectly. This allows circular compile-time dependency between parsers.
		/// </summary>
		/// <remarks>Avoid left recursion, which will result in a stack overflow at runtime.</remarks>
		public static IParser<T> Ref<T>(Func<IParser<T>> parserGenerator)
		{
			IParser<T>? parser = null;
			return Create(position => (parser ??= parserGenerator()).TryParse(position));
		}

		/// <summary>
		/// Succeeds with the specified value without advancing the text position.
		/// </summary>
		public static IParser<T> Success<T>(T value) => Create(position => ParseResult.Success(value, position));

		/// <summary>
		/// Fails if the specified predicate returns false for the successfully parsed value.
		/// </summary>
		public static IParser<T> Where<T>(this IParser<T> parser, Func<T, bool> predicate) =>
			Create(position => parser.TryParse(position).MapSuccess(result => predicate(result.Value) ? result : ParseResult.Failure<T>(position)));
	}
}
