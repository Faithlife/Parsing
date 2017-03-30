using System;
using Xunit;

namespace Faithlife.Parsing.Tests
{
	public class WhereTests
	{
		[Fact]
		public void TestWhere()
		{
			var parser = Parser.String("aba", StringComparison.OrdinalIgnoreCase).Where(x => x[0] != x[2]);
			parser.TryParse("x", 1).ShouldFail(1);
			parser.TryParse("xaba", 1).ShouldFail(1);
			parser.TryParse("xAba", 1).ShouldSucceed("Aba", 4);
			parser.TryParse("xabA", 1).ShouldSucceed("abA", 4);
			parser.TryParse("xAbA", 1).ShouldFail(1);
		}
	}
}
