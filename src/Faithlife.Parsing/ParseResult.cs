using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.FormattableString;

namespace Faithlife.Parsing
{
	/// <summary>
	/// Helper methods for parse results.
	/// </summary>
	public static class ParseResult
	{
		/// <summary>
		/// Creates a successful parse result.
		/// </summary>
		public static IParseResult<T> Success<T>(T value, TextPosition nextPosition) => new SuccessResult<T>(value, nextPosition);

		/// <summary>
		/// Creates a failed parse result.
		/// </summary>
		public static IParseResult<T> Failure<T>(TextPosition nextPosition) => new FailureResult<T>(nextPosition);

		/// <summary>
		/// Gets the parse result value on success, or the default value on failure.
		/// </summary>
		public static T GetValueOrDefault<T>(this IParseResult<T> result) => result.GetValueOrDefault(default);

		/// <summary>
		/// Gets the parse result value on success, or the default value on failure.
		/// </summary>
		public static T GetValueOrDefault<T>(this IParseResult<T> result, T defaultValue) => result.Success ? result.Value : defaultValue;

		/// <summary>
		/// Gets the named failures that were registered while parsing.
		/// </summary>
		public static IReadOnlyList<NamedFailure> GetNamedFailures(this IParseResult result) => result.NextPosition.GetNamedFailures();

		/// <summary>
		/// Maps a successful parse result into another parse result (success or failure).
		/// </summary>
		public static IParseResult<U> MapSuccess<T, U>(this IParseResult<T> result, Func<IParseResult<T>, IParseResult<U>> convert)
			=> result.Success ? convert(result) : Failure<U>(result.NextPosition);

		/// <summary>
		/// Creates a message for the parse result.
		/// </summary>
		/// <remarks>Displays where the parsing succeeded or failed, as well as any named failures if the parsing failed.</remarks>
		public static string ToMessage(this IParseResult result)
		{
			if (result.Success)
			{
				return Invariant($"success at {result.NextPosition.GetLineColumn()}");
			}
			else
			{
				var stringBuilder = new StringBuilder();

				stringBuilder.Append(Invariant($"failure at {result.NextPosition.GetLineColumn()}"));

				foreach (var namedFailureGroup in result.GetNamedFailures().Distinct()
					.GroupBy(x => x.Position.GetLineColumn())
					.OrderByDescending(x => x.Key.LineNumber).ThenByDescending(x => x.Key.ColumnNumber)
					.Select(x => new { Position = x.Key, Names = x.Select(y => y.Name).Distinct().ToArray() }))
				{
					stringBuilder.Append(Invariant($"; expected {string.Join(" or ", namedFailureGroup.Names)} at {namedFailureGroup.Position}"));
				}

				return stringBuilder.ToString();
			}
		}

		private sealed class SuccessResult<T> : IParseResult<T>
		{
			public SuccessResult(T value, TextPosition nextPosition)
			{
				Value = value;
				NextPosition = nextPosition;
			}

			public bool Success => true;

			public T Value { get; }

			public TextPosition NextPosition { get; }

			object IParseResult.Value => Value;
		}

		private sealed class FailureResult<T> : IParseResult<T>
		{
			public FailureResult(TextPosition nextPosition)
			{
				NextPosition = nextPosition;
			}

			public bool Success => false;

			public T Value => throw new ParseException(this);

			public TextPosition NextPosition { get; }

			object IParseResult.Value => Value;
		}
	}
}
