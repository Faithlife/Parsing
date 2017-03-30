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
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
		}

		/// <summary>
		/// The line number (one-based).
		/// </summary>
		public int LineNumber { get; }

		/// <summary>
		/// The column number (one-based).
		/// </summary>
		public int ColumnNumber { get; }

		/// <summary>
		/// Checks for equality.
		/// </summary>
		public bool Equals(LineColumn other)
		{
			return LineNumber == other.LineNumber && ColumnNumber == other.ColumnNumber;
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
			return (LineNumber << 8) ^ ColumnNumber;
		}

		/// <summary>
		/// Renders the line number and column number with a comma in between.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0},{1}", LineNumber, ColumnNumber);
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
	}
}
