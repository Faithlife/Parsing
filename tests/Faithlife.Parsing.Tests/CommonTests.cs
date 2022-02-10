using Xunit;

namespace Faithlife.Parsing.Tests;

public class CommonTests
{
	[Fact]
	public void ResultShouldAlwaysSucceed()
	{
		Parser.Success(true).TryParse("").ShouldSucceed(true, 0);
		Parser.Success(true).TryParse("x").ShouldSucceed(true, 0);
		Parser.Success(true).TryParse("xabc").ShouldSucceed(true, 0);
	}

	[Fact]
	public void EndShouldSucceedAtEnd()
	{
		Parser.Success(true).End().TryParse("x", 1).ShouldSucceed(true, 1);
	}

	[Fact]
	public void EndShouldFailNotAtEnd()
	{
		Parser.Success(true).End().TryParse("xabc", 1).ShouldFail(1);
	}

	[Fact]
	public void ParseThrowsOnFailure()
	{
		try
		{
			Parser.Success(true).End().Parse("xabc", 1);
			Assert.True(false);
		}
		catch (ParseException exception)
		{
			exception.Result.NextPosition.Index.ShouldBe(1);
		}
	}

	[Fact]
	public void NamedShouldNameFailure()
	{
		var namedFailure = Parser.String("abc").Named("epic").TryParse("xabc").GetNamedFailures().Single();
		namedFailure.Name.ShouldBe("epic");
		namedFailure.Position.Index.ShouldBe(0);
	}

	[Fact]
	public void PositionedShouldPositionSuccess()
	{
		var positioned = Parser.String("ab").Positioned().Parse("xabc", 1);
		positioned.Position.Index.ShouldBe(1);
		positioned.Value.ShouldBe("ab");
		positioned.Length.ShouldBe(2);
	}

	[Fact]
	public void LineColumnEquality()
	{
		var a1 = new LineColumn(1, 2);
		var a2 = new LineColumn(1, 2);
		var b = new LineColumn(2, 1);
		(a1 == a2).ShouldBe(true);
		(a1 != b).ShouldBe(true);
		Equals(a1, b).ShouldBe(false);
		a1.GetHashCode().ShouldBe(a2.GetHashCode());
	}

	[Fact]
	public void TextPositionEquality()
	{
		var values = Parser.String("ab").Positioned().Repeat(2).Parse("abab");
		Equals(values[0].Position, values[1].Position).ShouldBe(false);
		Equals(values[0].Position.GetHashCode(), values[1].Position.GetHashCode()).ShouldBe(false);
		(values[0].Position == values[1].Position).ShouldBe(false);
		(values[0].Position != values[1].Position).ShouldBe(true);
	}

	[Fact]
	public void ParseResult_GetValueOrDefault()
	{
		ParseResult.Success(1, default).GetValueOrDefault().ShouldBe(1);
		ParseResult.Success(1, default).GetValueOrDefault(2).ShouldBe(1);
		ParseResult.Failure<int>(default).GetValueOrDefault().ShouldBe(0);
		ParseResult.Failure<int>(default).GetValueOrDefault(2).ShouldBe(2);
	}

	[Fact]
	public void ParseResult_ToMessage()
	{
		Parser.Char('a').TryParse("a").ToMessage().ShouldBe("success at 1,2");
		Parser.Char('a').TryParse("b").ToMessage().ShouldBe("failure at 1,1");
		Parser.Char('a').Named("'a'").TryParse("b").ToMessage().ShouldBe("failure at 1,1; expected 'a' at 1,1");
	}
}
