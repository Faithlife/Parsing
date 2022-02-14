using System.Text.RegularExpressions;

namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Succeeds if the specified regular expression pattern matches the text.
	/// </summary>
	/// <remarks>The regular expression pattern is automatically anchored at the beginning
	/// of the text, but not at the end of the text. The parsed value is the successful Match.</remarks>
	public static IParser<Match> Regex(string pattern) => Regex(pattern, RegexOptions.CultureInvariant);

	/// <summary>
	/// Succeeds if the specified regular expression pattern matches the text.
	/// </summary>
	/// <remarks>The regular expression pattern is automatically anchored at the beginning
	/// of the text, but not at the end of the text. The parsed value is the successful Match.</remarks>
	public static IParser<Match> Regex(string pattern, RegexOptions regexOptions)
	{
		// turn off multiline mode for '^'; wrap pattern in non-capturing group to ensure ungrouped '|' works properly
		return new RegexParser(new Regex("(?-m:^)(?:" + pattern + ")", regexOptions));
	}

	private sealed class RegexParser : Parser<Match>
	{
		public RegexParser(Regex regex) => m_regex = regex;

		public override Match TryParse(ref TextPosition position, out bool success)
		{
			var inputIndex = position.Index;
			var inputText = position.Text;
			var match = m_regex.Match(inputText, inputIndex, inputText.Length - inputIndex);
			if (match.Success)
			{
				position = position.WithNextIndex(match.Length);
				success = true;
				return match;
			}

			success = false;
			return default!;
		}

		private readonly Regex m_regex;
	}
}
