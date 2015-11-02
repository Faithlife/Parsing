namespace Faithlife.Parsing
{
	public interface IParser<out T>
	{
		IParseResult<T> TryParse(TextPosition position);
	}
}
