namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<T> Named<T>(this IParser<T> parser, string name)
		{
			return Parser.Create(input =>
			{
				IResult<T> result = parser.TryParse(input);
				if (!result.Success)
					result.Remainder.Source.ReportNamedFailure(name, result);
				return result;
			});
		}
	}
}
