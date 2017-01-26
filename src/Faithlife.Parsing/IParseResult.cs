namespace Faithlife.Parsing
{
	/// <summary>
	/// The result of a parse attempt.
	/// </summary>
	/// <seealso cref="ParseResult" />
	public interface IParseResult
	{
		/// <summary>
		/// True if the parsing was successful.
		/// </summary>
		bool Success { get; }

		/// <summary>
		/// The parsed object instance. Throws a ParseException if the parsing was not successful.
		/// </summary>
		object Value { get; }

		/// <summary>
		/// The text position at the end of the parsed value.
		/// </summary>
		TextPosition NextPosition { get; }
	}

	/// <summary>
	/// The result of a parse attempt.
	/// </summary>
	/// <seealso cref="ParseResult" />
	public interface IParseResult<out T> : IParseResult
	{
		/// <summary>
		/// The parsed object instance. Throws a ParseException if the parsing was not successful.
		/// </summary>
		new T Value { get; }
	}
}
