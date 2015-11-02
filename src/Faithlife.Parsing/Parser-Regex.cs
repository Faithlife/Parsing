using System.Text.RegularExpressions;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<Match> Regex(string pattern)
		{
			return Regex(pattern, RegexOptions.None);
		}

		public static IParser<Match> Regex(string pattern, RegexOptions regexOptions)
		{
			Regex regex = new Regex("^(?:" + pattern + ")", regexOptions);

			return Create(position =>
			{
				int inputIndex = position.Index;
				string inputText = position.Text;
				Match match = regex.Match(inputText, inputIndex, inputText.Length - inputIndex);
				if (match.Success)
					return ParseResult.Success(match, position.WithIndex(match.Index + match.Length));

				return ParseResult.Failure<Match>(position);
			});
		}
	}
}
