namespace Faithlife.Parsing;

/// <summary>
/// A named parsing failure.
/// </summary>
public sealed class NamedFailure
{
	/// <summary>
	/// Creates an instance.
	/// </summary>
	public NamedFailure(string name, TextPosition position)
	{
		Name = name;
		Position = position;
	}

	/// <summary>
	/// The name of the failure, i.e. what was expected.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The position of the failure.
	/// </summary>
	public TextPosition Position { get; }
}
