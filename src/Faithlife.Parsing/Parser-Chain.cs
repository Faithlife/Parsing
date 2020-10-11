using System;

namespace Faithlife.Parsing
{
	public static partial class Parser
	{
		/// <summary>
		/// Chains a left-associative unary operator to the parser.
		/// </summary>
		public static IParser<TValue> ChainUnary<TValue, TOperator>(this IParser<TValue> parser, IParser<TOperator> opParser, Func<TOperator, TValue, TValue> apply)
		{
			return opParser.Then(Next).Or(parser);

			IParser<TValue> Next(TOperator outer) => opParser.Then(Next).Or(parser).Select(value => apply(outer, value));
		}

		/// <summary>
		/// Chains a left-associative binary operator to the parser.
		/// </summary>
		public static IParser<TValue> ChainBinary<TValue, TOperator>(this IParser<TValue> parser, IParser<TOperator> opParser, Func<TOperator, TValue, TValue, TValue> apply)
		{
			return parser.Then(Next);

			IParser<TValue> Next(TValue first) => opParser.Then(op => parser.Then(second => Next(apply(op, first, second)))).Or(Success(first));
		}
	}
}
