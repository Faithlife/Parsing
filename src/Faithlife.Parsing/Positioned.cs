namespace Faithlife.Parsing
{
	public sealed class Positioned<T>
	{
		public Positioned(T value, TextPosition position, int length)
		{
			m_value = value;
			m_position = position;
			m_length = length;
		}

		public T Value
		{
			get { return m_value; }
		}

		public TextPosition Position
		{
			get { return m_position; }
		}

		public int Length
		{
			get { return m_length; }
		}

		readonly T m_value;
		readonly TextPosition m_position;
		readonly int m_length;
	}
}
