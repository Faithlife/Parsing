using Xunit;

namespace Faithlife.Parsing.Tests;

public class ThenTests
{
	[Fact]
	public void TestSelectMany()
	{
		var parser =
			from open in Parser.Char('(')
			from node in Parser.AnyCharExcept(')').AtLeastOnce().String()
			from close in Parser.Char(')')
			where node != "fail"
			select node;
		parser.TryParse("x()x", 1).ShouldFail(2);
		parser.TryParse("x(f)x", 1).ShouldSucceed("f", 4);
		parser.TryParse("x(fail)x", 1).ShouldFail(1);
		parser.TryParse("x(failure)x", 1).ShouldSucceed("failure", 10);
	}

	[Fact]
	public void TestAppend()
	{
		Parser.Char('a').Repeat(2).Append(Parser.Char('b')).TryParse("aabc").ShouldSucceed(new[] { 'a', 'a', 'b' }, 3);
	}
}
