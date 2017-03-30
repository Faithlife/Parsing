namespace Faithlife.Parsing
{
	/// <summary>
	/// Wraps a text position and length around a parsed value.
	/// </summary>
	public sealed class Positioned<T>
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		public Positioned(T value, TextPosition position, int length)
		{
			Value = value;
			Position = position;
			Length = length;
		}

		/// <summary>
		/// The parsed value.
		/// </summary>
		public T Value { get; }

		/// <summary>
		/// The text position of the parsed value.
		/// </summary>
		public TextPosition Position { get; }

		/// <summary>
		/// The text length of the parsed value.
		/// </summary>
		public int Length { get; }
	}
}
