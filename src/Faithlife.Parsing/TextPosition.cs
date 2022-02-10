using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Faithlife.Parsing
{
	/// <summary>
	/// A position in the text being parsed.
	/// </summary>
	public struct TextPosition : IEquatable<TextPosition>
	{
		/// <summary>
		/// The entire text being parsed.
		/// </summary>
		public string Text => m_source.Text;

		/// <summary>
		/// The zero-based index into the text being parsed.
		/// </summary>
		public int Index => m_index;

		/// <summary>
		/// The line and column number of the text position.
		/// </summary>
		public LineColumn GetLineColumn() => m_source.GetPositionFromIndex(m_index);

		/// <summary>
		/// True if the text position is at the end of the text.
		/// </summary>
		public bool IsAtEnd() => m_index == m_source.Text.Length;

		/// <summary>
		/// Gets the character at the text position.
		/// </summary>
		/// <remarks>Throws IndexOutOfRangeException if the text position is at the end of the text.</remarks>
		[SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Legacy.")]
		public char GetCurrentChar() => m_source.Text[m_index];

		/// <summary>
		/// Creates a new text position at the next index.
		/// </summary>
		public TextPosition WithNextIndex() => new TextPosition(m_source, m_index + 1);

		/// <summary>
		/// Creates a new text position advanced the specified number of characters.
		/// </summary>
		public TextPosition WithNextIndex(int count) => new TextPosition(m_source, m_index + count);

		/// <summary>
		/// Checks for equality.
		/// </summary>
		public bool Equals(TextPosition other) => m_index == other.m_index && m_source == other.m_source;

		/// <summary>
		/// Checks for equality.
		/// </summary>
		public override bool Equals(object obj) => obj is TextPosition position && Equals(position);

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		public override int GetHashCode() => m_index;

		/// <summary>
		/// Checks for equality.
		/// </summary>
		public static bool operator ==(TextPosition first, TextPosition second) => first.Equals(second);

		/// <summary>
		/// Checks for equality.
		/// </summary>
		public static bool operator !=(TextPosition first, TextPosition second) => !(first == second);

		internal TextPosition(string text, int index)
			: this(new TextSource(text), index)
		{
		}

		internal ReadOnlyCollection<NamedFailure> GetNamedFailures() => m_source.GetNamedFailures();

		internal void ReportNamedFailure(string name, IParseResult result) => m_source.ReportNamedFailure(name, result);

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

			public string Text => m_text;

			public ReadOnlyCollection<NamedFailure> GetNamedFailures() => new ReadOnlyCollection<NamedFailure>(m_namedFailures);

			public void ReportNamedFailure(string name, IParseResult result)
			{
				var failureIndex = result.NextPosition.Index;

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

			[SuppressMessage("ReSharper", "RedundantEnumerableCastCall", Justification = "Cast needed for .NET Standard 2.0.")]
			public LineColumn GetPositionFromIndex(int index)
			{
				if (m_startOfLineIndices == null)
					m_startOfLineIndices = s_startOfLineRegex.Matches(m_text).Cast<Match>().Select(x => x.Index).ToArray();

				var lineIndex = Array.BinarySearch(m_startOfLineIndices, index);
				if (lineIndex < 0)
					lineIndex = ~lineIndex - 1;

				return new LineColumn(lineIndex + 1, index - m_startOfLineIndices[lineIndex] + 1);
			}

			private static readonly Regex s_startOfLineRegex = new Regex("^", RegexOptions.Multiline);

			private readonly string m_text;
			private readonly List<NamedFailure> m_namedFailures;
			private int[]? m_startOfLineIndices;
			private int m_firstFailureIndex;
			private int m_lastFailureIndex;
		}

		private readonly TextSource m_source;
		private readonly int m_index;
	}
}
