namespace Faithlife.Parsing;

/// <summary>
/// Parses text.
/// </summary>
/// <remarks>To parse text, call one of the <c>Parse</c> or <c>TryParse</c> overloads
/// on <see cref="Parser" />. To create a parser, assemble it from the many parsers provided
/// by <see cref="Parser" /> or derive a new parser from <see cref="Parser{T}" />.</remarks>
public interface IParser<out T>
{
	/// <summary>
	/// Attempts to parse the text at the specified position into an instance of type T.
	/// </summary>
	/// <remarks>To parse text, call one of the <c>Parse</c> or <c>TryParse</c> overloads
	/// on <see cref="Parser" />.</remarks>
	IParseResult<T> TryParse(TextPosition position);

	/// <summary>
	/// Attempts to parse the text at the specified position into an instance of type T.
	/// </summary>
	/// <remarks>To parse text, call one of the <c>Parse</c> or <c>TryParse</c> overloads
	/// on <see cref="Parser" />.</remarks>
	T TryParse(bool skip, ref TextPosition position, out bool success);
}
