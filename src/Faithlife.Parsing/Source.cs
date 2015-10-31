using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Faithlife.Parsing
{
	internal class Source
	{
		public string Text
		{
			get { return m_text; }
		}

		internal Source(string text)
		{
			m_text = text;
			m_namedFailures = new List<NamedFailure>();
		}

		internal ReadOnlyCollection<NamedFailure> GetNamedFailures()
		{
			return new ReadOnlyCollection<NamedFailure>(m_namedFailures);
		}

		internal void ReportNamedFailure(string name, IResult result)
		{
			int failureIndex = result.Remainder.Index;

			if (m_namedFailures.Count == 0 || failureIndex > m_lastFailureIndex || failureIndex <= m_firstFailureIndex)
			{
				if (failureIndex > m_firstFailureIndex)
				{
					m_lastFailureIndex = failureIndex;
					m_namedFailures.Clear();
				}

				m_firstFailureIndex = failureIndex;
				m_namedFailures.Add(new NamedFailure(name, result.Remainder));
			}
		}

		internal Position GetPositionFromIndex(int index)
		{
			if (m_startOfLineIndices == null)
				m_startOfLineIndices = s_startOfLineRegex.Matches(m_text).OfType<Match>().Select(x => x.Index).ToArray();

			int lineIndex = Array.BinarySearch(m_startOfLineIndices, index);
			if (lineIndex < 0)
				lineIndex = ~lineIndex - 1;

			return new Position(index, lineIndex + 1, index - m_startOfLineIndices[lineIndex] + 1);
		}

		static readonly Regex s_startOfLineRegex = new Regex("^", RegexOptions.Multiline);

		readonly string m_text;
		readonly List<NamedFailure> m_namedFailures;
		int[] m_startOfLineIndices;
		int m_firstFailureIndex;
		int m_lastFailureIndex;
	}
}
