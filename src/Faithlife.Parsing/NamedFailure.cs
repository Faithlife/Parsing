namespace Faithlife.Parsing
{
	public sealed class NamedFailure
	{
		public NamedFailure(string name, TextPosition position)
		{
			m_name = name;
			m_position = position;
		}

		public string Name
		{
			get { return m_name; }
		}

		public TextPosition Position
		{
			get { return m_position; }
		}

		readonly string m_name;
		readonly TextPosition m_position;
	}
}
