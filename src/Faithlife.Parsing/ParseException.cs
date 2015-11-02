using System;

namespace Faithlife.Parsing
{
	public sealed class ParseException : Exception
	{
		public ParseException(IParseResult result)
			: base(result.ToMessage())
		{
			m_result = result;
		}

		public IParseResult Result
		{
			get { return m_result; }
		}

		readonly IParseResult m_result;
	}
}
