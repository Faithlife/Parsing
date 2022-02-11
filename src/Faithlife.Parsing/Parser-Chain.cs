namespace Faithlife.Parsing;

public static partial class Parser
{
	/// <summary>
	/// Chains a left-associative unary operator to the parser.
	/// </summary>
	public static IParser<TValue> ChainUnary<TValue, TOperator>(this IParser<TValue> parser, IParser<TOperator> opParser, Func<TOperator, TValue, TValue> apply) =>
		opParser.Many().Then(parser, (ops, value) => ops.Reverse().Aggregate(value, (v, x) => apply(x, v)));

	/// <summary>
	/// Chains a left-associative binary operator to the parser.
	/// </summary>
	public static IParser<TValue> ChainBinary<TValue, TOperator>(this IParser<TValue> parser, IParser<TOperator> opParser, Func<TOperator, TValue, TValue, TValue> apply) =>
		parser.Then(opParser.Then(parser).Many(), (value, ops) => ops.Aggregate(value, (v, x) => apply(x.Item1, v, x.Item2)));
}
