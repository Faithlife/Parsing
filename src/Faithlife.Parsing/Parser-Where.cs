using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<T> Where<T>(this IParser<T> parser, Func<T, bool> predicate)
		{
			return Parser.Create(input => parser.TryParse(input).IfSuccess(result => predicate(result.Value) ? result : Result.Failure<T>(input)));
		}
	}
}
