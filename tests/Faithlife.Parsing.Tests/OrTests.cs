using Xunit;

namespace Faithlife.Parsing.Tests
{
	public class OrTests
	{
		[Fact]
		public void OrShouldFailWhenBothFail()
		{
			Parser.String("abc").Or(Parser.String("xyz")).TryParse("").ShouldFail(0);
			Parser.String("abc").Or(Parser.String("xyz")).TryParse("abbd").ShouldFail(0);
			Parser.String("abc").Or(Parser.String("xyz")).TryParse("xyyz").ShouldFail(0);
		}

		[Fact]
		public void OrShouldSucceedWhenFirstSucceeds()
		{
			Parser.String("abc").Or(Parser.String("xyz")).TryParse("abcd").ShouldSucceed("abc", 3);
		}

		[Fact]
		public void OrShouldSucceedWhenSecondSucceeds()
		{
			Parser.String("abc").Or(Parser.String("xyz")).TryParse("xyzz").ShouldSucceed("xyz", 3);
		}
	}
}
