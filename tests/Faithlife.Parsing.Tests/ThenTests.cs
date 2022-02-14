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

	[Fact]
	public void TestAppendSkipped()
	{
		Parser.Char('a').Repeat(2).Append(Parser.Char('b')).Success(true).TryParse("aabc").ShouldSucceed(true, 3);
	}

	[Fact]
	public void ThenTuple()
	{
		var tuple2 = Parser.Char('a').Then(Parser.Char('b'));
		tuple2.TryParse("ab").ShouldSucceed(('a', 'b'));
		var tuple3 = tuple2.Then(Parser.Char('c'));
		tuple3.TryParse("abc").ShouldSucceed(('a', 'b', 'c'));
		var tuple4 = tuple3.Then(Parser.Char('d'));
		tuple4.TryParse("abcd").ShouldSucceed(('a', 'b', 'c', 'd'));
		var tuple5 = tuple4.Then(Parser.Char('e'));
		tuple5.TryParse("abcde").ShouldSucceed(('a', 'b', 'c', 'd', 'e'));
		var tuple6 = tuple5.Then(Parser.Char('f'));
		tuple6.TryParse("abcdef").ShouldSucceed(('a', 'b', 'c', 'd', 'e', 'f'));
		var tuple7 = tuple6.Then(Parser.Char('g'));
		tuple7.TryParse("abcdefg").ShouldSucceed(('a', 'b', 'c', 'd', 'e', 'f', 'g'));
		var tuple8 = tuple7.Then(Parser.Char('h'));
		tuple8.TryParse("abcdefgh").ShouldSucceed(('a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'));
	}

	[Fact]
	public void TestSelectSkipped()
	{
		var parser = Parser.Char('a').Many().String().Select(x => x.Length);
		parser.TryParse("aabc").ShouldSucceed(2);
		parser.Success(true).TryParse("aabc").ShouldSucceed(true);
	}
}
