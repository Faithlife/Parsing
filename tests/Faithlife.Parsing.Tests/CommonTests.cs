using System.Linq;
using Xunit;

namespace Faithlife.Parsing.Tests
{
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
			positioned.Length.ShouldBe(2);
		}
	}
}
