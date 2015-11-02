using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<T> End<T>(this IParser<T> parser)
		{
			return Create(position => parser.TryParse(position)
				.MapSuccess(result => result.NextPosition.IsAtEnd() ? result : ParseResult.Failure<T>(result.NextPosition)));
		}

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

		public static IParser<Positioned<T>> Positioned<T>(this IParser<T> parser)
		{
			return Create(position => parser.TryParse(position)
				.MapSuccess(result => ParseResult.Success(new Positioned<T>(result.Value, position, result.NextPosition.Index - position.Index), result.NextPosition)));
		}

		/// <summary>
		/// Refer to another parser indirectly. This allows circular compile-time dependency between parsers.
		/// </summary>
		/// <remarks>Avoid left recursion, which will result in a stack overflow at runtime.</remarks>
		public static IParser<T> Ref<T>(Func<IParser<T>> parserGenerator)
		{
			IParser<T> parser = null;
			return Create(position => (parser ?? (parser = parserGenerator())).TryParse(position));
		}

		public static IParser<T> Success<T>(T value)
		{
			return Create(position => ParseResult.Success(value, position));
		}

		public static IParser<T> Where<T>(this IParser<T> parser, Func<T, bool> predicate)
		{
			return Create(position => parser.TryParse(position).MapSuccess(result => predicate(result.Value) ? result : ParseResult.Failure<T>(position)));
		}
	}
}
