using System;
using System.Globalization;

namespace Faithlife.Parsing
{
	/// <summary>
	/// A line and column number.
	/// </summary>
	public struct LineColumn : IEquatable<LineColumn>
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		public LineColumn(int lineNumber, int columnNumber)
		{
			m_lineNumber = lineNumber;
			m_columnNumber = columnNumber;
		}

		/// <summary>
		/// The line number (one-based).
		/// </summary>
		public int LineNumber => m_lineNumber;

		/// <summary>
		/// The column number (one-based).
		/// </summary>
		public int ColumnNumber => m_columnNumber;

		/// <summary>
		/// Checks for equality.
		/// </summary>
		public bool Equals(LineColumn other)
		{
			return m_lineNumber == other.m_lineNumber && m_columnNumber == other.m_columnNumber;
		}

		/// <summary>
		/// Checks for equality.
		/// </summary>
		public override bool Equals(object other)
		{
			return other is LineColumn && Equals((LineColumn) other);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		public override int GetHashCode()
		{
			return (m_lineNumber << 8) ^ m_columnNumber;
		}

		/// <summary>
		/// Renders the line number and column number with a comma in between.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0},{1}", m_lineNumber, m_columnNumber);
		}

		/// <summary>
		/// Checks for equality.
		/// </summary>
		public static bool operator ==(LineColumn first, LineColumn second)
		{
			return first.Equals(second);
		}

		/// <summary>
		/// Checks for inequality.
		/// </summary>
		public static bool operator !=(LineColumn first, LineColumn second)
		{
			return !(first == second);
		}

		readonly int m_lineNumber;
		readonly int m_columnNumber;
	}
}
