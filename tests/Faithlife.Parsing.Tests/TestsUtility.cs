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

		public static void ShouldSucceed<T>(this IParseResult<T> actual, T expected, int? remainderIndex = null)
		{
			Assert.True(actual.Success, "parse failed");
			if (expected is IEnumerable sequence && !(expected is string))
				Assert.True(sequence.Cast<object>().SequenceEqual(((IEnumerable) actual.Value!).Cast<object>()));
			else
				Assert.Equal(expected, actual.Value);
			if (remainderIndex != null)
				Assert.Equal(remainderIndex.Value, actual.NextPosition.Index);
		}

		public static void ShouldSucceed<T>(this IParseResult<T> actual, Func<T, bool> expected, int? remainderIndex = null)
		{
			Assert.True(actual.Success, "parse failed");
			Assert.True(expected(actual.Value), "unexpected value");
			if (remainderIndex != null)
				Assert.Equal(remainderIndex.Value, actual.NextPosition.Index);
		}

		public static void ShouldFail<T>(this IParseResult<T> actual, int? remainderIndex = null)
		{
			Assert.False(actual.Success, "parse succeeded");
			if (remainderIndex != null)
				Assert.Equal(remainderIndex.Value, actual.NextPosition.Index);
		}
	}
}
