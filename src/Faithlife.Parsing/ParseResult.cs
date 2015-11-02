using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Faithlife.Parsing
{
	public static class ParseResult
	{
		public static IParseResult<T> Success<T>(T value, TextPosition nextPosition)
		{
			return new SuccessResult<T>(value, nextPosition);
		}

		public static IParseResult<T> Failure<T>(TextPosition nextPosition)
		{
			return new FailureResult<T>(nextPosition);
		}

		public static T GetValueOrDefault<T>(this IParseResult<T> result)
		{
			return result.GetValueOrDefault(default(T));
		}

		public static T GetValueOrDefault<T>(this IParseResult<T> result, T defaultValue)
		{
			return result.Success ? result.Value : defaultValue;
		}

		public static IReadOnlyList<NamedFailure> GetNamedFailures(this IParseResult result)
		{
			return result.NextPosition.GetNamedFailures();
		}

		public static IParseResult<U> MapSuccess<T, U>(this IParseResult<T> result, Func<IParseResult<T>, IParseResult<U>> convert)
		{
			return result.Success ? convert(result) : Failure<U>(result.NextPosition);
		}

		public static string ToMessage(this IParseResult result)
		{
			if (result.Success)
			{
				return string.Format(CultureInfo.InvariantCulture, "success at {0}", result.NextPosition.GetLineColumn());
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();

				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "failure at {0}", result.NextPosition.GetLineColumn());

				foreach (var namedFailureGroup in result.GetNamedFailures().Distinct()
					.GroupBy(x => x.Position.GetLineColumn())
					.OrderByDescending(x => x.Key.LineNumber).ThenByDescending(x => x.Key.ColumnNumber)
					.Select(x => new { Position = x.Key, Names = x.Select(y => y.Name).Distinct().ToArray() }))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture,
						"; expected {0} at {1}", string.Join(" or ", namedFailureGroup.Names), namedFailureGroup.Position);
				}

				return stringBuilder.ToString();
			}
		}

		private sealed class SuccessResult<T> : IParseResult<T>
		{
			public SuccessResult(T value, TextPosition nextPosition)
			{
				m_value = value;
				m_nextPosition = nextPosition;
			}

			public bool Success
			{
				get { return true; }
			}

			public T Value
			{
				get { return m_value; }
			}

			public TextPosition NextPosition
			{
				get { return m_nextPosition; }
			}

			object IParseResult.Value
			{
				get { return Value; }
			}

			readonly T m_value;
			readonly TextPosition m_nextPosition;
		}

		private sealed class FailureResult<T> : IParseResult<T>
		{
			public FailureResult(TextPosition nextPosition)
			{
				m_nextPosition = nextPosition;
			}

			public bool Success
			{
				get { return false; }
			}

			public T Value
			{
				get { throw new ParseException(this); }
			}

			public TextPosition NextPosition
			{
				get { return m_nextPosition; }
			}

			object IParseResult.Value
			{
				get { return Value; }
			}

			readonly TextPosition m_nextPosition;
		}
	}
}
