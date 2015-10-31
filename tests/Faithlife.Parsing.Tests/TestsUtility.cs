using System;
using System.Collections;
using System.Linq;
using Xunit;

namespace Faithlife.Parsing.Tests
{
	public static class TestsUtility
	{
		public static void ShouldBe<T>(this T actual, T expected)
		{
			Assert.Equal(expected, actual);
		}

		public static void ShouldSucceed<T>(this IResult<T> actual, T expected, int remainderIndex)
		{
			Assert.True(actual.Success, "parse failed");
			IEnumerable sequence = expected as IEnumerable;
			if (sequence != null)
				Assert.True(sequence.Cast<object>().SequenceEqual(((IEnumerable) actual.Value).Cast<object>()));
			else
				Assert.Equal(expected, actual.Value);
			AssertRemainderIndex(actual, remainderIndex);
		}

		public static void ShouldSucceed<T>(this IResult<T> actual, Func<T, bool> expected, int remainderIndex)
		{
			Assert.True(actual.Success, "parse failed");
			Assert.True(expected(actual.Value), "unexpected value");
			AssertRemainderIndex(actual, remainderIndex);
		}

		public static void ShouldFail<T>(this IResult<T> actual, int remainderIndex)
		{
			Assert.False(actual.Success, "parse succeeded");
			AssertRemainderIndex(actual, remainderIndex);
		}

		private static void AssertRemainderIndex<T>(IResult<T> actual, int remainderIndex)
		{
			Assert.Equal(remainderIndex, actual.Remainder.Index);
		}
	}
}
