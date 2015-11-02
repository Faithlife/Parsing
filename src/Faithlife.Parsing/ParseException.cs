using System;

namespace Faithlife.Parsing
{
	/// <summary>
	/// Thrown when parsing fails.
	/// </summary>
	public sealed class ParseException : Exception
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		public ParseException(IParseResult result)
			: base(result.ToMessage())
		{
			m_result = result;
		}

		/// <summary>
		/// The failed parse result.
		/// </summary>
		public IParseResult Result
		{
			get { return m_result; }
		}

		readonly IParseResult m_result;
	}
}
