namespace Faithlife.Parsing;

/// <summary>
/// Thrown when parsing fails.
/// </summary>
public sealed class ParseException : Exception
{
	/// <summary>
	/// Creates an instance.
	/// </summary>
	public ParseException(IParseResult result)
		: base(result.ToMessage())
	{
		Result = result;
	}

	/// <summary>
	/// The failed parse result.
	/// </summary>
	public IParseResult Result { get; }
}
