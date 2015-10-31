using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Faithlife.Parsing
{
	public static class Result
	{
		public static IResult<T> Success<T>(T value, Input remainder)
		{
			return new SuccessResult<T>(value, remainder);
		}

		public static IResult<T> Failure<T>(Input remainder)
		{
			return new FailureResult<T>(remainder);
		}

		public static IReadOnlyList<NamedFailure> GetNamedFailures(this IResult result)
		{
			return result.Remainder.Source.GetNamedFailures();
		}

		public static IResult<U> IfSuccess<T, U>(this IResult<T> result, Func<IResult<T>, IResult<U>> convert)
		{
			return result.Success ? convert(result) : Result.Failure<U>(result.Remainder);
		}

		public static IResult<T> IfFailure<T>(this IResult<T> result, Func<IResult<T>, IResult<T>> convert)
		{
			return result.Success ? result : convert(result);
		}

		public static string ToMessage(this IResult result)
		{
			if (result.Success)
			{
				return string.Format(CultureInfo.InvariantCulture, "success at {0}", result.Remainder.Position);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();

				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "failure at {0}", result.Remainder.Position);

				foreach (var namedFailureGroup in result.GetNamedFailures().Distinct()
					.GroupBy(x => x.Remainder.Position)
					.OrderByDescending(x => x.Key.LineNumber).ThenByDescending(x => x.Key.ColumnNumber)
					.Select(x => new { Position = x.Key, Names = x.Select(y => y.Name).Distinct().ToArray() }))
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture,
						"; expected {0} at {1}", string.Join(" or ", namedFailureGroup.Names), namedFailureGroup.Position);
				}

				return stringBuilder.ToString();
			}
		}

		private sealed class SuccessResult<T> : IResult<T>
		{
			public SuccessResult(T value, Input remainder)
			{
				m_value = value;
				m_remainder = remainder;
			}

			public bool Success
			{
				get { return true; }
			}

			public T Value
			{
				get { return m_value; }
			}

			public Input Remainder
			{
				get { return m_remainder; }
			}

			object IResult.Value
			{
				get { return Value; }
			}

			readonly T m_value;
			readonly Input m_remainder;
		}

		private sealed class FailureResult<T> : IResult<T>
		{
			public FailureResult(Input remainder)
			{
				m_remainder = remainder;
			}

			public bool Success
			{
				get { return false; }
			}

			public T Value
			{
				get { throw new InvalidOperationException("Parse failed; Value not available."); }
			}

			public Input Remainder
			{
				get { return m_remainder; }
			}

			object IResult.Value
			{
				get { return Value; }
			}

			readonly Input m_remainder;
		}
	}
}
