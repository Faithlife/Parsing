using System.Globalization;
using System.Text.RegularExpressions;
using Xunit;

namespace Faithlife.Parsing.Tests;

public partial class RegexTests
{
	[Fact]
	public void RegexShouldFailIfNoMatch()
	{
		Parser.Regex("abc").TryParse("").ShouldFail(0);
		Parser.Regex("abc").TryParse("a").ShouldFail(0);
		Parser.Regex("abc").TryParse("Abc").ShouldFail(0);
	}

	[Fact]
	public void RegexShouldSucceedIfMatch()
	{
		Parser.Regex("abc").TryParse("xabcd", 1).ShouldSucceed(x => x.ToString() == "abc", 4);
	}

	[Fact]
	public void RegexShouldFailBeforeMatch()
	{
		Parser.Regex("abc").TryParse("aaabcd", 1).ShouldFail(1);
	}

	[Fact]
	public void RegexShouldFailAfterMatch()
	{
		Parser.Regex("abc").TryParse("abcd", 1).ShouldFail(1);
	}

	[Fact]
	public void RegexShouldRespectOptions()
	{
		Parser.Regex("Abc").TryParse("xabcd", 1).ShouldFail(1);
		Parser.Regex("Abc", RegexOptions.IgnoreCase).TryParse("xabcd", 1).ShouldSucceed(x => x.ToString() == "abc", 4);
	}

	[Fact]
	public void GeneratedRegex()
	{
		Parser.Regex(AbcOrdinal()).TryParse("xabcd", 1).ShouldFail(1);
		Parser.Regex(AbcOrdinal()).TryParse("xAbcd", 1).ShouldSucceed(x => (x.Index, x.Value) == (1, "Abc"), 4);
		Parser.Regex(AbcIgnoreCase()).TryParse("xabcd", 1).ShouldSucceed(x => (x.Index, x.Value) == (1, "abc"), 4);
	}

	[GeneratedRegex("^Abc", RegexOptions.CultureInvariant)]
	private static partial Regex AbcOrdinal();

	[GeneratedRegex("^Abc", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
	private static partial Regex AbcIgnoreCase();
}
