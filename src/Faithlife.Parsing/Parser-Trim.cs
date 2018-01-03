namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Succeeds if the specified parser also succeeds beforehand (ignoring its result).
		/// </summary>
		public static IParser<T> PrecededBy<T, U>(this IParser<T> parser, IParser<U> precededBy) => precededBy.Then(_ => parser);

		/// <summary>
		/// Succeeds if the specified parser also succeeds afterward (ignoring its result).
		/// </summary>
		public static IParser<T> FollowedBy<T, U>(this IParser<T> parser, IParser<U> followedBy) => parser.Then(followedBy.Success);

		/// <summary>
		/// Succeeds if the specified parsers succeed beforehand and afterward (ignoring their results).
		/// </summary>
		public static IParser<T> Bracketed<T, U, V>(this IParser<T> parser, IParser<U> precededBy, IParser<V> followedBy) => parser.PrecededBy(precededBy).FollowedBy(followedBy);

		/// <summary>
		/// Succeeds if the specified parser succeeds, ignoring any whitespace characters beforehand.
		/// </summary>
		public static IParser<T> TrimStart<T>(this IParser<T> parser) => parser.PrecededBy(WhiteSpace.Many());

		/// <summary>
		/// Succeeds if the specified parser succeeds, ignoring any whitespace characters afterward.
		/// </summary>
		public static IParser<T> TrimEnd<T>(this IParser<T> parser) => parser.FollowedBy(WhiteSpace.Many());

		/// <summary>
		/// Succeeds if the specified parser succeeds, ignoring any whitespace characters beforehand or afterward.
		/// </summary>
		public static IParser<T> Trim<T>(this IParser<T> parser) => parser.TrimStart().TrimEnd();
	}
}
