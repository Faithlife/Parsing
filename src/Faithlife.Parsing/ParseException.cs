using System;

namespace Faithlife.Parsing
{
	public class ParseException : Exception
	{
		public ParseException(IResult result)
			: this(result, null)
		{
		}

		public ParseException(IResult result, Exception innerException)
			: base(result.ToMessage(), innerException)
		{
			m_result = result;
		}

		public IResult Result
		{
			get { return m_result; }
		}

		readonly IResult m_result;
	}
}
