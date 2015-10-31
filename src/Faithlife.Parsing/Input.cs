using System;

namespace Faithlife.Parsing
{
	public struct Input : IEquatable<Input>
	{
		public string Text
		{
			get { return m_source.Text; }
		}

		public int Index
		{
			get { return m_index; }
		}

		public Position Position
		{
			get { return m_source.GetPositionFromIndex(m_index); }
		}

		public bool AtEnd
		{
			get { return m_index == m_source.Text.Length; }
		}

		public char Current
		{
			get { return m_source.Text[m_index]; }
		}

		public Input Advance()
		{
			return new Input(m_source, m_index + 1);
		}

		public Input Advance(int count)
		{
			return new Input(m_source, m_index + count);
		}

		public Input AtIndex(int index)
		{
			return new Input(m_source, index);
		}

		public bool Equals(Input other)
		{
			return m_index == other.m_index && m_source == other.m_source;
		}

		public override bool Equals(object other)
		{
			return other is Input && Equals((Input) other);
		}

		public override int GetHashCode()
		{
			return m_index;
		}

		public static bool operator ==(Input first, Input second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(Input first, Input second)
		{
			return !(first == second);
		}

		internal Input(Source source, int index)
		{
			m_source = source;
			m_index = index;
		}

		internal Source Source
		{
			get { return m_source; }
		}

		readonly Source m_source;
		readonly int m_index;
	}
}
