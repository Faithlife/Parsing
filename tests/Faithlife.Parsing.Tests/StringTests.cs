using Xunit;

namespace Faithlife.Parsing.Tests
{
	public class StringTests
	{
		[Fact]
		public void EmptyStringShouldAlwaysSucceed()
		{
			var parser = Parser.String("");
			parser.TryParse("").ShouldSucceed("", 0);
			parser.TryParse("a").ShouldSucceed("", 0);
		}

		[Fact]
		public void TestSingleCharacter()
		{
			var parser = Parser.String("a");
			parser.TryParse("").ShouldFail(0);
			parser.TryParse("a").ShouldSucceed("a", 1);
			parser.TryParse("ab").ShouldSucceed("a", 1);
			parser.TryParse("b").ShouldFail(0);
		}

		[Fact]
		public void TestMultipleCharacters()
		{
			var parser = Parser.String("abc");
			parser.TryParse("").ShouldFail(0);
			parser.TryParse("a").ShouldFail(0);
			parser.TryParse("ab").ShouldFail(0);
			parser.TryParse("abc").ShouldSucceed("abc", 3);
			parser.TryParse("abcd").ShouldSucceed("abc", 3);
			parser.TryParse("b").ShouldFail(0);
		}

		[Fact]
		public void TestIgnoreCase()
		{
			var parser = Parser.StringIgnoreCase("Abc");
			parser.TryParse("").ShouldFail(0);
			parser.TryParse("a").ShouldFail(0);
			parser.TryParse("ab").ShouldFail(0);
			parser.TryParse("abc").ShouldSucceed("abc", 3);
			parser.TryParse("Abc").ShouldSucceed("Abc", 3);
			parser.TryParse("aBc").ShouldSucceed("aBc", 3);
			parser.TryParse("abCd").ShouldSucceed("abC", 3);
			parser.TryParse("b").ShouldFail(0);
		}

		[Fact]
		public void TestConvertToCharsAndBack()
		{
			IParser<string> parser = Parser.String("abc").Chars().String();
			parser.TryParse("abcd").ShouldSucceed("abc", 3);
		}
	}
}
