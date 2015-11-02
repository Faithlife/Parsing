using System;

namespace Faithlife.Parsing
{
	public struct LineColumn : IEquatable<LineColumn>
	{
		public LineColumn(int lineNumber, int columnNumber)
		{
			m_lineNumber = lineNumber;
			m_columnNumber = columnNumber;
		}

		public int LineNumber
		{
			get { return m_lineNumber; }
		}

		public int ColumnNumber
		{
			get { return m_columnNumber; }
		}

		public bool Equals(LineColumn other)
		{
			return m_lineNumber == other.m_lineNumber && m_columnNumber == other.m_columnNumber;
		}

		public override bool Equals(object other)
		{
			return other is LineColumn && Equals((LineColumn) other);
		}

		public override int GetHashCode()
		{
			return (m_lineNumber << 8) ^ m_columnNumber;
		}

		public override string ToString()
		{
			return m_lineNumber + "," + m_columnNumber;
		}

		public static bool operator ==(LineColumn first, LineColumn second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(LineColumn first, LineColumn second)
		{
			return !(first == second);
		}

		readonly int m_lineNumber;
		readonly int m_columnNumber;
	}
}
