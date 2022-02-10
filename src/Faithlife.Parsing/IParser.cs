namespace Faithlife.Parsing;

/// <summary>
/// Parses text.
/// </summary>
/// <seealso cref="Parser" />
public interface IParser<out T>
{
	/// <summary>
	/// Attempts to parse the text at the specified position into an instance of type T.
	/// </summary>
	IParseResult<T> TryParse(TextPosition position);
}
