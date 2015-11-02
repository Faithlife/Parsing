namespace Faithlife.Parsing
{
	public interface IParseResult
	{
		bool Success { get; }

		TextPosition NextPosition { get; }

		object Value { get; }
	}

	public interface IParseResult<out T> : IParseResult
	{
		new T Value { get; }
	}
}
