using Xunit;

namespace Faithlife.Parsing.Tests
{
	public class RepeatTests
	{
		[Fact]
		public void OnceShouldFailIfNoMatch()
		{
			Parser.String("abc").Once().TryParse("").ShouldFail(0);
			Parser.String("abc").Once().TryParse("a").ShouldFail(0);
			Parser.String("abc").Once().TryParse("b").ShouldFail(0);
		}

		[Fact]
		public void OnceShouldSucceedIfMatch()
		{
			Parser.String("abc").Once().TryParse("abc").ShouldSucceed(new[] { "abc" }, 3);
			Parser.String("abc").Once().TryParse("abcd").ShouldSucceed(new[] { "abc" }, 3);
		}

		[Fact]
		public void OnceShouldOnlyEatOne()
		{
			Parser.String("abc").Once().TryParse("abcabc").ShouldSucceed(new[] { "abc" }, 3);
		}

		[Fact]
		public void AtMostShouldAlwaysSucceed()
		{
			var parser = Parser.String("abc").AtMost(2);
			parser.TryParse("").ShouldSucceed(new string[0], 0);
			parser.TryParse("a").ShouldSucceed(new string[0], 0);
			parser.TryParse("abc").ShouldSucceed(new[] { "abc" }, 3);
			parser.TryParse("abcd").ShouldSucceed(new[] { "abc" }, 3);
			parser.TryParse("abcabc").ShouldSucceed(new[] { "abc", "abc" }, 6);
			parser.TryParse("abcabcd").ShouldSucceed(new[] { "abc", "abc" }, 6);
			parser.TryParse("abcabcabc").ShouldSucceed(new[] { "abc", "abc" }, 6);
			parser.TryParse("abcabcabcd").ShouldSucceed(new[] { "abc", "abc" }, 6);
			parser.TryParse("b").ShouldSucceed(new string[0], 0);
		}

		[Fact]
		public void AtMostOnceShouldAlwaysSucceed()
		{
			Parser.String("abc").AtMostOnce().TryParse("bcabc").ShouldSucceed(new string[0], 0);
		}

		[Fact]
		public void AtMostOnceShouldOnlyEatOne()
		{
			Parser.String("abc").AtMostOnce().TryParse("abcabc").ShouldSucceed(new[] { "abc" }, 3);
		}

		[Fact]
		public void ManyShouldAlwaysSucceed()
		{
			var parser = Parser.String("abc").Many();
			parser.TryParse("").ShouldSucceed(new string[0], 0);
			parser.TryParse("ab").ShouldSucceed(new string[0], 0);
			parser.TryParse("abc").ShouldSucceed(new[] { "abc" }, 3);
			parser.TryParse("abcd").ShouldSucceed(new[] { "abc" }, 3);
			parser.TryParse("abcabcd").ShouldSucceed(new[] { "abc", "abc" }, 6);
			parser.TryParse("abcabcabc").ShouldSucceed(new[] { "abc", "abc", "abc" }, 9);
			parser.TryParse("b").ShouldSucceed(new string[0], 0);
		}

		[Fact]
		public void AtLeastShouldFailOnNone()
		{
			Parser.String("abc").AtLeast(2).TryParse("").ShouldFail(0);
			Parser.String("abc").AtLeast(2).TryParse("abd").ShouldFail(0);
		}

		[Fact]
		public void AtLeastShouldEatNothingOnFailure()
		{
			Parser.String("abc").AtLeast(2).TryParse("abc").ShouldFail(0);
		}

		[Fact]
		public void AtLeastShouldSucceedIfEnough()
		{
			var parser = Parser.String("abc").AtLeast(2);
			parser.TryParse("abcabc").ShouldSucceed(new[] { "abc", "abc" }, 6);
			parser.TryParse("abcabcd").ShouldSucceed(new[] { "abc", "abc" }, 6);
			parser.TryParse("abcabcabc").ShouldSucceed(new[] { "abc", "abc", "abc" }, 9);
			parser.TryParse("abcabcabcd").ShouldSucceed(new[] { "abc", "abc", "abc" }, 9);
		}
	}
}
