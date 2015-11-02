using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Faithlife.Parsing.Json
{
	public static class JsonParsers
	{
		public static IParser<long> JsonInteger = Parser
			.Regex(@"-?(0|[1-9][0-9]*)(?![0-9.eE])", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)
			.Trim()
			.Select(match => long.Parse(match.ToString(), CultureInfo.InvariantCulture))
			.Named("integer");

		public static IParser<double> JsonDouble = Parser
			.Regex(@"-?(0|[1-9][0-9]*)(.[0-9]+)?([eE][-+]?[0-9]+)?(?![0-9.eE])", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)
			.Trim()
			.Select(match => double.Parse(match.ToString(), CultureInfo.InvariantCulture))
			.Named("double");

		public static IParser<object> JsonNumber =
			JsonInteger.Select(x => (object) x)
			.Or(JsonDouble.Select(x => (object) x));

		public static IParser<string> JsonString = Parser
			.Regex(@"""(\\(?:[""\\/bfnrt]|u[0-9a-fA-F]{4})|[^""\\]+)*""", RegexOptions.CultureInvariant)
			.Trim()
			.Select(match => string.Concat(match.Groups[1].Captures.Cast<Capture>().Select(x => UnescapeString(x.ToString()))))
			.Named("string");

		public static IParser<bool> JsonBoolean =
			Token("true").Success(true)
			.Or(Token("false").Success(false))
			.Named("boolean");

		public static IParser<object> JsonNull = Token("null").Success((object) null);

		public static IParser<IReadOnlyList<T>> JsonArrayOf<T>(this IParser<T> parser)
		{
			return parser
				.Delimited(Token(","))
				.OrEmpty()
				.Bracketed(Token("["), Token("]"))
				.Named("array");
		}

		public static IParser<KeyValuePair<string, T>> JsonPropertyOf<T>(this IParser<T> parser)
		{
			return
				from name in JsonString
				from colon in Token(":")
				from value in parser
				select new KeyValuePair<string, T>(name, value);
		}

		public static IParser<T> JsonPropertyNamed<T>(this IParser<T> parser, string name)
		{
			return parser.JsonPropertyNamed(name, StringComparison.Ordinal);
		}

		public static IParser<T> JsonPropertyNamed<T>(this IParser<T> parser, string name, StringComparison comparison)
		{
			return parser.JsonPropertyOf().Where(x => string.Equals(x.Key, name, comparison)).Select(x => x.Value);
		}

		public static IParser<IReadOnlyList<T>> JsonObjectOf<T>(this IParser<T> parser)
		{
			return parser
				.Delimited(Token(","))
				.OrEmpty()
				.Bracketed(Token("{"), Token("}"))
				.Named("object");
		}

		public static IParser<IReadOnlyList<object>> JsonArray = Parser.Ref(() => JsonValue).JsonArrayOf();

		public static IParser<IReadOnlyList<KeyValuePair<string, object>>> JsonObject = Parser.Ref(() => JsonValue).JsonPropertyOf().JsonObjectOf();

		public static IParser<object> JsonValue = Parser.Or(
			JsonString,
			JsonNumber,
			JsonObject,
			JsonArray,
			JsonBoolean.Select(x => (object) x),
			JsonNull);

		private static IParser<string> Token(string value)
		{
			return Parser.String(value).Trim().Named("'" + value + "'");
		}

		private static string UnescapeString(string value)
		{
			if (value[0] != '\\')
				return value;

			switch (value[1])
			{
			case '"':
				return "\"";
			case '\\':
				return "\\";
			case '/':
				return "/";
			case 'b':
				return "\b";
			case 'f':
				return "\f";
			case 'n':
				return "\n";
			case 'r':
				return "\r";
			case 't':
				return "\t";
			case 'u':
				return new string((char) int.Parse(value.Substring(2), NumberStyles.HexNumber), 1);
			default:
				throw new InvalidOperationException();
			}
		}
	}
}
