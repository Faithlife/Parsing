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

			return Parser.Create(input =>
			{
				int inputIndex = input.Index;
				string inputText = input.Text;
				Match match = regex.Match(inputText, inputIndex, inputText.Length - inputIndex);
				if (match.Success)
					return Result.Success(match, input.AtIndex(match.Index + match.Length));

				return Result.Failure<Match>(input);
			});
		}
	}
}
