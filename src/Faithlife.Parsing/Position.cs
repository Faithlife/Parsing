using System;

namespace Faithlife.Parsing
{
	public struct Position : IEquatable<Position>
	{
		public Position(int index, int lineNumber, int columnNumber)
		{
			m_index = index;
			m_lineNumber = lineNumber;
			m_columnNumber = columnNumber;
		}

		public int Index
		{
			get { return m_index; }
		}

		public int LineNumber
		{
			get { return m_lineNumber; }
		}

		public int ColumnNumber
		{
			get { return m_columnNumber; }
		}

		public bool Equals(Position other)
		{
			return m_index == other.m_index;
		}

		public override bool Equals(object other)
		{
			return other is Position && Equals((Position) other);
		}

		public override int GetHashCode()
		{
			return m_index;
		}

		public override string ToString()
		{
			return m_lineNumber + "," + m_columnNumber;
		}

		public static bool operator ==(Position first, Position second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(Position first, Position second)
		{
			return !(first == second);
		}

		readonly int m_index;
		readonly int m_lineNumber;
		readonly int m_columnNumber;
	}
}
