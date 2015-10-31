namespace Faithlife.Parsing
{
	public class Positioned<T>
	{
		public Positioned(T value, Input input)
		{
			m_value = value;
			m_input = input;
		}

		public T Value
		{
			get { return m_value; }
		}

		public Input Input
		{
			get { return m_input; }
		}

		readonly T m_value;
		readonly Input m_input;
	}
}
