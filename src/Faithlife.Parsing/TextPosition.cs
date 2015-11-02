using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Faithlife.Parsing
{
	public struct TextPosition : IEquatable<TextPosition>
	{
		public string Text
		{
			get { return m_source.Text; }
		}

		public int Index
		{
			get { return m_index; }
		}

		public LineColumn GetLineColumn()
		{
			return m_source.GetPositionFromIndex(m_index);
		}

		public bool IsAtEnd()
		{
			return m_index == m_source.Text.Length;
		}

		public char GetCurrentChar()
		{
			return m_source.Text[m_index];
		}

		public TextPosition WithIndex(int index)
		{
			return new TextPosition(m_source, index);
		}

		public TextPosition WithNextIndex()
		{
			return WithIndex(m_index + 1);
		}

		public TextPosition WithNextIndex(int count)
		{
			return WithIndex(m_index + count);
		}

		public bool Equals(TextPosition other)
		{
			return m_index == other.m_index && m_source == other.m_source;
		}

		public override bool Equals(object other)
		{
			return other is TextPosition && Equals((TextPosition) other);
		}

		public override int GetHashCode()
		{
			return m_index;
		}

		public static bool operator ==(TextPosition first, TextPosition second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(TextPosition first, TextPosition second)
		{
			return !(first == second);
		}

		internal TextPosition(string text, int index)
			: this(new TextSource(text), index)
		{
		}

		internal ReadOnlyCollection<NamedFailure> GetNamedFailures()
		{
			return m_source.GetNamedFailures();
		}

		internal void ReportNamedFailure(string name, IParseResult result)
		{
			m_source.ReportNamedFailure(name, result);
		}

		private TextPosition(TextSource source, int index)
		{
			m_source = source;
			m_index = index;
		}

		private sealed class TextSource
		{
			public TextSource(string text)
			{
				m_text = text;
				m_namedFailures = new List<NamedFailure>();
			}

			public string Text
			{
				get { return m_text; }
			}

			public ReadOnlyCollection<NamedFailure> GetNamedFailures()
			{
				return new ReadOnlyCollection<NamedFailure>(m_namedFailures);
			}

			public void ReportNamedFailure(string name, IParseResult result)
			{
				int failureIndex = result.NextPosition.Index;

				if (m_namedFailures.Count == 0 || failureIndex > m_lastFailureIndex || failureIndex <= m_firstFailureIndex)
				{
					if (failureIndex > m_firstFailureIndex)
					{
						m_lastFailureIndex = failureIndex;
						m_namedFailures.Clear();
					}

					m_firstFailureIndex = failureIndex;
					m_namedFailures.Add(new NamedFailure(name, result.NextPosition));
				}
			}

			public LineColumn GetPositionFromIndex(int index)
			{
				if (m_startOfLineIndices == null)
					m_startOfLineIndices = s_startOfLineRegex.Matches(m_text).OfType<Match>().Select(x => x.Index).ToArray();

				int lineIndex = Array.BinarySearch(m_startOfLineIndices, index);
				if (lineIndex < 0)
					lineIndex = ~lineIndex - 1;

				return new LineColumn(lineIndex + 1, index - m_startOfLineIndices[lineIndex] + 1);
			}

			static readonly Regex s_startOfLineRegex = new Regex("^", RegexOptions.Multiline);

			readonly string m_text;
			readonly List<NamedFailure> m_namedFailures;
			int[] m_startOfLineIndices;
			int m_firstFailureIndex;
			int m_lastFailureIndex;
		}

		readonly TextSource m_source;
		readonly int m_index;
	}
}
