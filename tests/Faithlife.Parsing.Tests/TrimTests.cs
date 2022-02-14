using Xunit;

namespace Faithlife.Parsing.Tests;

public class TrimTests
{
	[Fact]
	public void TrimStart()
	{
		Parser.Letter.TrimStart().End().TryParse(" x").ShouldSucceed('x', 2);
		Parser.Letter.TrimStart().End().TryParse("x ").ShouldFail(1);
	}

	[Fact]
	public void TrimEnd()
	{
		Parser.Letter.TrimEnd().End().TryParse("x ").ShouldSucceed('x', 2);
		Parser.Letter.TrimEnd().End().TryParse(" x").ShouldFail(0);
	}
}
