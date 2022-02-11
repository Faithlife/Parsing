using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Faithlife.Parsing.Json;

/// <summary>
/// JSON parsers. (Primarily for expository purposes; prefer specialized JSON parsers for production.)
/// </summary>
public static class JsonParsers
{
	/// <summary>
	/// Parses a JSON number into a 64-bit integer. Fails if the number has a decimal point or exponent.
	/// </summary>
	public static readonly IParser<long> JsonInteger = Parser
		.Regex(@"-?(0|[1-9][0-9]*)(?![0-9.eE])", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)
		.Trim()
		.Select(match => long.Parse(match.ToString(), CultureInfo.InvariantCulture))
		.Named("integer");

	/// <summary>
	/// Parses a JSON number into a double-precision floating-point number.
	/// </summary>
	public static readonly IParser<double> JsonDouble = Parser
		.Regex(@"-?(0|[1-9][0-9]*)(.[0-9]+)?([eE][-+]?[0-9]+)?(?![0-9.eE])", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)
		.Trim()
		.Select(match => double.Parse(match.ToString(), CultureInfo.InvariantCulture))
		.Named("double");

	/// <summary>
	/// Parses a JSON number into a 64-bit integer if possible; otherwise uses a double-precision floating-point number.
	/// </summary>
	public static readonly IParser<object> JsonNumber =
		JsonInteger.Select(x => (object) x)
			.Or(JsonDouble.Select(x => (object) x));

	/// <summary>
	/// Parses a JSON string.
	/// </summary>
	[SuppressMessage("ReSharper", "RedundantEnumerableCastCall", Justification = "Cast needed for .NET Standard 2.0.")]
	public static readonly IParser<string> JsonString = Parser
		.Regex(@"""(\\(?:[""\\/bfnrt]|u[0-9a-fA-F]{4})|[^""\\]+)*""", RegexOptions.CultureInvariant)
		.Trim()
		.Select(match => string.Concat(match.Groups[1].Captures.Cast<Capture>().Select(x => UnescapeString(x.ToString()))))
		.Named("string");

	/// <summary>
	/// Parses a JSON Boolean.
	/// </summary>
	public static readonly IParser<bool> JsonBoolean =
		Token("true").Success(true)
			.Or(Token("false").Success(false))
			.Named("boolean");

	/// <summary>
	/// Parses a JSON null.
	/// </summary>
	public static readonly IParser<object?> JsonNull = Token("null").Success((object?) null);

	/// <summary>
	/// Parses an array of JSON values, each of which is parsed with the specified parser.
	/// </summary>
	public static IParser<IReadOnlyList<T>> JsonArrayOf<T>(this IParser<T> parser)
	{
		return parser
			.Delimited(Token(","))
			.OrEmpty()
			.Bracketed(Token("["), Token("]"))
			.Named("array");
	}

	/// <summary>
	/// Parses a JSON object property (i.e. name, colon, and value). The property value is parsed with the specified parser.
	/// </summary>
	public static IParser<KeyValuePair<string, T>> JsonPropertyOf<T>(this IParser<T> parser)
	{
		return
			from name in JsonString
			from colon in Token(":")
			from value in parser
			select new KeyValuePair<string, T>(name, value);
	}

	/// <summary>
	/// Parses a JSON object property into its value. Fails if the property name doesn't match the specified name.
	/// </summary>
	public static IParser<T> JsonPropertyNamed<T>(this IParser<T> parser, string name)
		=> parser.JsonPropertyNamed(name, StringComparison.Ordinal);

	/// <summary>
	/// Parses a JSON object property into its value. Fails if the property name doesn't match the specified name.
	/// </summary>
	public static IParser<T> JsonPropertyNamed<T>(this IParser<T> parser, string name, StringComparison comparison)
		=> parser.JsonPropertyOf().Where(x => string.Equals(x.Key, name, comparison)).Select(x => x.Value);

	/// <summary>
	/// Parses a JSON object from its properties. The specified parser must parse each entire JSON property (name, colon, and value).
	/// </summary>
	public static IParser<IReadOnlyList<T>> JsonObjectOf<T>(this IParser<T> parser)
	{
		return parser
			.Delimited(Token(","))
			.OrEmpty()
			.Bracketed(Token("{"), Token("}"))
			.Named("object");
	}

	/// <summary>
	/// Parses a JSON array of arbitrary JSON values.
	/// </summary>
	public static readonly IParser<IReadOnlyList<object?>> JsonArray = Parser.Ref(() => JsonValue!).JsonArrayOf();

	/// <summary>
	/// Parses a JSON object of arbitrary JSON property values.
	/// </summary>
	public static readonly IParser<IReadOnlyList<KeyValuePair<string, object?>>> JsonObject = Parser.Ref(() => JsonValue!).JsonPropertyOf().JsonObjectOf();

	/// <summary>
	/// Parses an arbitrary JSON value.
	/// </summary>
	/// <remarks>The resulting object is a null, Boolean, String, Int64, Double, IReadOnlyList{Object},
	/// or IReadOnlyList{KeyValuePair{string,object}}.</remarks>
	public static readonly IParser<object?> JsonValue = Parser.Or(
		JsonString,
		JsonNumber,
		JsonObject,
		JsonArray,
		JsonBoolean.Select(x => (object) x),
		JsonNull);

	private static IParser<string> Token(string value) => Parser.String(value, StringComparison.Ordinal).Trim().Named("'" + value + "'");

	private static string UnescapeString(string value)
	{
		if (value[0] != '\\')
			return value;

		return value[1] switch
		{
			'"' => "\"",
			'\\' => "\\",
			'/' => "/",
			'b' => "\b",
			'f' => "\f",
			'n' => "\n",
			'r' => "\r",
			't' => "\t",
#if NETSTANDARD2_0
			'u' => new string((char) int.Parse(value.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture), 1),
#else
			'u' => new string((char) int.Parse(value.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture), 1),
#endif
			_ => throw new InvalidOperationException(),
		};
	}
}
