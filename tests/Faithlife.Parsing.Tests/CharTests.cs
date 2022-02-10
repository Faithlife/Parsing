using Xunit;

namespace Faithlife.Parsing.Tests;

public class CharTests
{
	[Fact]
	public void ConstantShouldFailOnEmpty()
	{
		Parser.Char('a').TryParse("").ShouldFail(0);
	}

	[Fact]
	public void ConstantShouldSucceedOnGoodChar()
	{
		Parser.Char('a').TryParse("a").ShouldSucceed('a', 1);
		Parser.Char('a').TryParse("ab").ShouldSucceed('a', 1);
	}

	[Fact]
	public void ConstantShouldFailOnBadChar()
	{
		Parser.Char('a').TryParse("b").ShouldFail(0);
	}

	[Fact]
	public void PredicateShouldFailOnEmpty()
	{
		Parser.Char(ch => ch is >= 'a' and <= 'z').TryParse("").ShouldFail(0);
	}

	[Fact]
	public void PredicateShouldSucceedOnGoodChar()
	{
		Parser.Char(ch => ch is >= 'a' and <= 'z').TryParse("a").ShouldSucceed('a', 1);
		Parser.Char(ch => ch is >= 'a' and <= 'z').TryParse("ab").ShouldSucceed('a', 1);
		Parser.Char(ch => ch is >= 'a' and <= 'z').TryParse("b").ShouldSucceed('b', 1);
	}

	[Fact]
	public void PredicateShouldFailOnBadChar()
	{
		Parser.Char(ch => ch is >= 'a' and <= 'z').TryParse("1").ShouldFail(0);
	}

	[Fact]
	public void AnyCharShouldFailOnEmpty()
	{
		Parser.AnyChar.TryParse("").ShouldFail(0);
	}

	[Fact]
	public void AnyCharShouldSucceedOnAnyChar()
	{
		Parser.AnyChar.TryParse("a").ShouldSucceed('a', 1);
		Parser.AnyChar.TryParse("1").ShouldSucceed('1', 1);
	}

	[Fact]
	public void AnyCharExceptShouldFailOnEmpty()
	{
		Parser.AnyCharExcept('a').TryParse("").ShouldFail(0);
	}

	[Fact]
	public void AnyCharExceptShouldSucceedOnGoodChar()
	{
		Parser.AnyCharExcept('a').TryParse("b").ShouldSucceed('b', 1);
	}

	[Fact]
	public void AnyCharExceptShouldFailOnBadChar()
	{
		Parser.AnyCharExcept('a').TryParse("a").ShouldFail(0);
		Parser.AnyCharExcept('a').TryParse("ab").ShouldFail(0);
	}
}
