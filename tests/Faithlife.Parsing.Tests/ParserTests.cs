using Xunit;

namespace Faithlife.Parsing.Tests
{
	public class ParserTests
	{
		[Fact]
		public void ReturnShouldAlwaysSucceed()
		{
			Parser.Return(true).TryParse("").ShouldSucceed(true, 0);
			Parser.Return(true).TryParse("x").ShouldSucceed(true, 0);
			Parser.Return(true).TryParse("xabc").ShouldSucceed(true, 0);
		}

		[Fact]
		public void EndShouldSucceedAtEnd()
		{
			Parser.Return(true).End().TryParse("x", 1).ShouldSucceed(true, 1);
		}

		[Fact]
		public void EndShouldFailNotAtEnd()
		{
			Parser.Return(true).End().TryParse("xabc", 1).ShouldFail(1);
		}
	}
}
