namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Succeeds if the specified parser also succeeds beforehand (ignoring its result).
	/// </summary>
	public static IParser<TValue> PrecededBy<TValue, TPreceding>(this IParser<TValue> parser, IParser<TPreceding> precededBy)
	{
		if (parser is null)
			throw new ArgumentNullException(nameof(parser));
		return precededBy.Then(parser, (_, value) => value);
	}

	/// <summary>
	/// Succeeds if the specified parser also succeeds afterward (ignoring its result).
	/// </summary>
	public static IParser<TValue> FollowedBy<TValue, TFollowing>(this IParser<TValue> parser, IParser<TFollowing> followedBy)
	{
		if (followedBy is null)
			throw new ArgumentNullException(nameof(followedBy));
		return parser.Then(followedBy, (value, _) => value);
	}

	/// <summary>
	/// Succeeds if the specified parsers succeed beforehand and afterward (ignoring their results).
	/// </summary>
	public static IParser<TValue> Bracketed<TValue, TPreceding, TFollowing>(this IParser<TValue> parser, IParser<TPreceding> precededBy, IParser<TFollowing> followedBy) => parser.PrecededBy(precededBy).FollowedBy(followedBy);

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
