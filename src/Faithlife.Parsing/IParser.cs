namespace Faithlife.Parsing
{
	public interface IParser<out T>
	{
		IResult<T> TryParse(Input input);
	}
}
