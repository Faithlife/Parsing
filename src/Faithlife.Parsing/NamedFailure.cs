namespace Faithlife.Parsing
{
	public class NamedFailure
	{
		public NamedFailure(string name, Input remainder)
		{
			m_name = name;
			m_remainder = remainder;
		}

		public string Name
		{
			get { return m_name; }
		}

		public Input Remainder
		{
			get { return m_remainder; }
		}

		readonly string m_name;
		readonly Input m_remainder;
	}
}
