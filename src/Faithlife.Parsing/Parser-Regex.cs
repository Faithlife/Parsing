using System.Text.RegularExpressions;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Succeeds if the specified regular expression pattern matches the text.
		/// </summary>
		/// <remarks>The regular expression pattern is automatically anchored at the beginning
		/// of the text, but not at the end of the text. The parsed value is the successful Match.</remarks>
		public static IParser<Match> Regex(string pattern)
		{
			return Regex(pattern, RegexOptions.None);
		}

		/// <summary>
		/// Succeeds if the specified regular expression pattern matches the text.
		/// </summary>
		/// <remarks>The regular expression pattern is automatically anchored at the beginning
		/// of the text, but not at the end of the text. The parsed value is the successful Match.</remarks>
		public static IParser<Match> Regex(string pattern, RegexOptions regexOptions)
		{
			// turn off multiline mode for '^'; wrap pattern in non-capturing group to ensure ungrouped '|' works properly
			Regex regex = new Regex("(?-m:^)(?:" + pattern + ")", regexOptions);

			return Create(position =>
			{
				int inputIndex = position.Index;
				string inputText = position.Text;
				Match match = regex.Match(inputText, inputIndex, inputText.Length - inputIndex);
				if (match.Success)
					return ParseResult.Success(match, position.WithNextIndex(match.Length));

				return ParseResult.Failure<Match>(position);
			});
		}
	}
}
