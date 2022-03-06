namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Succeeds only at the end of the text.
	/// </summary>
	public static IParser<T> End<T>(this IParser<T> parser) => new ThenEndParser<T>(parser);

	private sealed class ThenEndParser<T> : Parser<T>
	{
		public ThenEndParser(IParser<T> parser) => m_parser = parser;

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(skip, ref position, out success);
			if (!success || position.IsAtEnd())
				return value;

			success = false;
			return default!;
		}

		private readonly IParser<T> m_parser;
	}

	/// <summary>
	/// Succeeds with the specified value only at the end of the text.
	/// </summary>
	public static IParser<T> End<T>(T value) => new EndParser<T>(value);

	private sealed class EndParser<T> : Parser<T>
	{
		public EndParser(T value) => m_value = value;

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			success = position.IsAtEnd();
			return success ? m_value : default!;
		}

		private readonly T m_value;
	}

	/// <summary>
	/// Reports a named failure with the specified name if the parser fails.
	/// </summary>
	public static IParser<T> Named<T>(this IParser<T> parser, string name) => new NamedParser<T>(parser, name);

	private sealed class NamedParser<T> : Parser<T>
	{
		public NamedParser(IParser<T> parser, string name) => (m_parser, m_name) = (parser, name);

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var value = m_parser.TryParse(skip, ref position, out success);
			if (!success)
				position.ReportNamedFailure(m_name);
			return value;
		}

		private readonly IParser<T> m_parser;
		private readonly string m_name;
	}

	/// <summary>
	/// Wraps the text position and length around a successfully parsed value.
	/// </summary>
	public static IParser<Positioned<T>> Positioned<T>(this IParser<T> parser) => new PositionedParser<T>(parser);

	private sealed class PositionedParser<T> : Parser<Positioned<T>>
	{
		public PositionedParser(IParser<T> parser) => m_parser = parser;

		public override Positioned<T> TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var startPosition = position;
			var value = m_parser.TryParse(skip, ref position, out success);
			return success ? new Positioned<T>(value, startPosition, position.Index - startPosition.Index) : default!;
		}

		private readonly IParser<T> m_parser;
	}

	/// <summary>
	/// Refers to another parser indirectly. This allows circular compile-time dependency between parsers.
	/// </summary>
	/// <remarks>Avoid left recursion, which will result in a stack overflow at runtime.</remarks>
	public static IParser<T> Ref<T>(Func<IParser<T>> parserGenerator) => new RefParser<T>(parserGenerator);

	private sealed class RefParser<T> : Parser<T>
	{
		public RefParser(Func<IParser<T>> parserGenerator) => m_parserGenerator = parserGenerator;

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			m_parser ??= m_parserGenerator();
			return m_parser.TryParse(skip, ref position, out success);
		}

		private readonly Func<IParser<T>> m_parserGenerator;
		private IParser<T>? m_parser;
	}

	/// <summary>
	/// Succeeds with the specified value without advancing the text position.
	/// </summary>
	public static IParser<T> Success<T>(T value) => new SuccessParser<T>(value);

	private sealed class SuccessParser<T> : Parser<T>
	{
		public SuccessParser(T value) => m_value = value;

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			success = true;
			return m_value;
		}

		private readonly T m_value;
	}

	/// <summary>
	/// Always fails.
	/// </summary>
	public static IParser<T> Failure<T>() => FailureParser<T>.Instance;

	private sealed class FailureParser<T> : Parser<T>
	{
		public static readonly FailureParser<T> Instance = new();

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			success = false;
			return default!;
		}
	}

	/// <summary>
	/// Fails if the specified predicate returns false for the successfully parsed value.
	/// </summary>
	public static IParser<T> Where<T>(this IParser<T> parser, Func<T, bool> predicate) => new WhereParser<T>(parser, predicate);

	private sealed class WhereParser<T> : Parser<T>
	{
		public WhereParser(IParser<T> parser, Func<T, bool> predicate) => (m_parser, m_predicate) = (parser, predicate);

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var startPosition = position;

			var value = m_parser.TryParse(skip: false, ref position, out success);
			if (!success || m_predicate(value))
				return value;

			position = startPosition;
			success = false;
			return default!;
		}

		private readonly IParser<T> m_parser;
		private readonly Func<T, bool> m_predicate;
	}

	/// <summary>
	/// Fails if the parser succeeds, and succeeds with the default value if it fails.
	/// </summary>
	public static IParser<T> Not<T>(this IParser<T> parser) => new NotParser<T>(parser);

	private sealed class NotParser<T> : Parser<T>
	{
		public NotParser(IParser<T> parser) => m_parser = parser;

		public override T TryParse(bool skip, ref TextPosition position, out bool success)
		{
			var endPosition = position;
			m_parser.TryParse(skip: true, ref endPosition, out success);
			success = !success;
			return default!;
		}

		private readonly IParser<T> m_parser;
	}
}
