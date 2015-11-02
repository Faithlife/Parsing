namespace Faithlife.Parsing
{
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
			m_name = name;
			m_position = position;
		}

		/// <summary>
		/// The name of the failure, i.e. what was expected.
		/// </summary>
		public string Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// The position of the failure.
		/// </summary>
		public TextPosition Position
		{
			get { return m_position; }
		}

		readonly string m_name;
		readonly TextPosition m_position;
	}
}
