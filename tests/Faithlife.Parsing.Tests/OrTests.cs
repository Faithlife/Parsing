using System.Collections.Generic;
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

		[Fact]
		public void OrShouldPreferFirstSuccess()
		{
			Parser.String("abc").Or(Parser.String("ab")).TryParse("abcd").ShouldSucceed("abc", 3);
			Parser.String("ab").Or(Parser.String("abc")).TryParse("abcd").ShouldSucceed("ab", 2);
		}

		[Fact]
		public void OrShouldPreferFirstNonEmptySuccess()
		{
			Parser.Success("x").Or(Parser.Success("y")).TryParse("abcd").ShouldSucceed("x", 0);
			Parser.Success("x").Or(Parser.String("abc")).TryParse("abcd").ShouldSucceed("abc", 3);
		}

		[Fact]
		public void OrDefault()
		{
			Parser.String("xyz").OrDefault().TryParse("abcd").ShouldSucceed(default(string), 0);
			Parser.String("xyz").OrDefault("zyx").TryParse("abcd").ShouldSucceed("zyx", 0);
		}

		[Fact]
		public void OrEmpty()
		{
			Parser.String("xyz").Chars().OrEmpty().TryParse("abcd").ShouldSucceed(new char[0], 0);
			Parser.String("xyz").Chars().Select(x => (IReadOnlyCollection<char>) x).OrEmpty().TryParse("abcd").ShouldSucceed(new char[0], 0);
			Parser.String("xyz").Chars().Select(x => (IEnumerable<char>) x).OrEmpty().TryParse("abcd").ShouldSucceed(new char[0], 0);
		}
	}
}
