namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<Positioned<T>> Positioned<T>(this IParser<T> parser)
		{
			return Parser.Create(input => parser.TryParse(input).IfSuccess(result => Result.Success(new Positioned<T>(result.Value, input), result.Remainder)));
		}
	}
}
