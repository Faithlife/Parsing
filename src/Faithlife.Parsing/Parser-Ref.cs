using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		public static IParser<T> Ref<T>(Func<IParser<T>> parserGenerator)
		{
			// TODO: detect left recursion
			IParser<T> parser = null;
			return Parser.Create(input => (parser ?? (parser = parserGenerator())).TryParse(input));
		}
	}
}
