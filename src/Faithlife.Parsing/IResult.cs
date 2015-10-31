namespace Faithlife.Parsing
{
	public interface IResult
	{
		bool Success { get; }

		Input Remainder { get; }

		object Value { get; }
	}

	public interface IResult<out T> : IResult
	{
		new T Value { get; }
	}
}
