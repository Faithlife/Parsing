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
			m_value = value;
			m_position = position;
			m_length = length;
		}

		/// <summary>
		/// The parsed value.
		/// </summary>
		public T Value
		{
			get { return m_value; }
		}

		/// <summary>
		/// The text position of the parsed value.
		/// </summary>
		public TextPosition Position
		{
			get { return m_position; }
		}

		/// <summary>
		/// The text length of the parsed value.
		/// </summary>
		public int Length
		{
			get { return m_length; }
		}

		readonly T m_value;
		readonly TextPosition m_position;
		readonly int m_length;
	}
}
