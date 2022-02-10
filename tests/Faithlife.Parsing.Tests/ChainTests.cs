using System.Globalization;
using Xunit;

namespace Faithlife.Parsing.Tests;

public class ChainTests
{
	[Theory]
	[InlineData("2 + 3", 5)]
	[InlineData("2 * 3", 6)]
	[InlineData("-1", -1)]
	[InlineData("2 + 3 * 4", 14)]
	[InlineData("-(2 + 3) * 4", -20)]
	[InlineData("-1 - -4 / 2", 1)]
	public void EvaluateTests(string text, int value)
	{
		Evaluate(text).ShouldBe(value);
	}

	private static int Evaluate(string text)
	{
		return Eval(Parse(text));

		static int Eval(ExpressionNode node) =>
			node.Value switch
			{
				"-" => node.Children.Count == 1 ? -Eval(node.Children[0]) : Eval(node.Children[0]) - Eval(node.Children[1]),
				"*" => Eval(node.Children[0]) * Eval(node.Children[1]),
				"/" => Eval(node.Children[0]) / Eval(node.Children[1]),
				"+" => Eval(node.Children[0]) + Eval(node.Children[1]),
				_ => int.Parse(node.Value, CultureInfo.InvariantCulture),
			};
	}

	private static ExpressionNode Parse(string text) => Expression.End().Parse(text);

	private static IParser<ExpressionNode> Name { get; } =
		Parser.Regex(@"[A-Za-z_][A-Za-z0-9_]*").Select(x => new ExpressionNode(x.Value));

	private static IParser<ExpressionNode> Number { get; } =
		Parser.Regex(@"[0-9]+").Select(x => new ExpressionNode(x.Value));

	private static IParser<string> Op(string op) =>
		Parser.String(op, StringComparison.Ordinal).Trim();

	private static IParser<ExpressionNode> Expression { get; } =
		Parser.Ref(() => Expression!).Bracketed(Op("("), Op(")")).Or(Name).Or(Number)
			.ChainUnary(Op("-").Trim(), (x, y) => new ExpressionNode(x, y))
			.ChainBinary(Op("*").Or(Op("/")).Trim(), (x, y, z) => new ExpressionNode(x, y, z))
			.ChainBinary(Op("+").Or(Op("-")).Trim(), (x, y, z) => new ExpressionNode(x, y, z));

	private class ExpressionNode
	{
		public ExpressionNode(string value, params ExpressionNode[] children) => (Value, Children) = (value, children);
		public string Value { get; }
		public IReadOnlyList<ExpressionNode> Children { get; }
	}
}
