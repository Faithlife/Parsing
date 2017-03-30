using System.Collections.Generic;
using System.Linq;
using Faithlife.Parsing.Json;
using Xunit;

namespace Faithlife.Parsing.Tests.Json
{
	public class JsonTests
	{
		[Fact]
		public void JsonIntegerSuccess()
		{
			JsonParsers.JsonInteger.TryParse("0").ShouldSucceed(0, 1);
			JsonParsers.JsonInteger.TryParse(" 0 ").ShouldSucceed(0, 3);
			JsonParsers.JsonInteger.TryParse("1234").ShouldSucceed(1234, 4);
			JsonParsers.JsonInteger.TryParse("-1234").ShouldSucceed(-1234, 5);
		}

		[Fact]
		public void JsonIntegerFailure()
		{
			JsonParsers.JsonInteger.TryParse("").ShouldFail(0);
			JsonParsers.JsonInteger.TryParse("01").ShouldFail(0);
			JsonParsers.JsonInteger.TryParse("1.0").ShouldFail(0);
			JsonParsers.JsonInteger.TryParse("1e1").ShouldFail(0);
			JsonParsers.JsonDouble.TryParse("1e").ShouldFail(0);
			JsonParsers.JsonDouble.TryParse("1.").ShouldFail(0);
		}

		[Fact]
		public void JsonDoubleSuccess()
		{
			JsonParsers.JsonDouble.TryParse("0").ShouldSucceed(0.0, 1);
			JsonParsers.JsonDouble.TryParse(" 0 ").ShouldSucceed(0.0, 3);
			JsonParsers.JsonDouble.TryParse("1234").ShouldSucceed(1234.0, 4);
			JsonParsers.JsonDouble.TryParse("-1234").ShouldSucceed(-1234.0, 5);
			JsonParsers.JsonDouble.TryParse("1").ShouldSucceed(1.0, 1);
			JsonParsers.JsonDouble.TryParse("1.0").ShouldSucceed(1.0, 3);
			JsonParsers.JsonDouble.TryParse("1e1").ShouldSucceed(10.0, 3);
			JsonParsers.JsonDouble.TryParse("-3.14e+10").ShouldSucceed(-3.14e+10, 9);
			JsonParsers.JsonDouble.TryParse("-3.14e-10").ShouldSucceed(-3.14e-10, 9);
			JsonParsers.JsonDouble.TryParse("3.14e10").ShouldSucceed(3.14e+10, 7);
		}

		[Fact]
		public void JsonDoubleFailure()
		{
			JsonParsers.JsonDouble.TryParse("").ShouldFail(0);
			JsonParsers.JsonDouble.TryParse("01").ShouldFail(0);
			JsonParsers.JsonDouble.TryParse("1e").ShouldFail(0);
			JsonParsers.JsonDouble.TryParse("1.").ShouldFail(0);
		}

		[Fact]
		public void JsonNumberSuccess()
		{
			JsonParsers.JsonNumber.TryParse("0").ShouldSucceed(0L, 1);
			JsonParsers.JsonNumber.TryParse("0.0").ShouldSucceed(0.0, 3);
		}

		[Fact]
		public void JsonStringSuccess()
		{
			JsonParsers.JsonString.TryParse("\"\"").ShouldSucceed("", 2);
			JsonParsers.JsonString.TryParse("\"hello\"").ShouldSucceed("hello", 7);
			JsonParsers.JsonString.TryParse("\"\\\"\\\\\\/\"").ShouldSucceed("\"\\/");
			JsonParsers.JsonString.TryParse("\"\\b\\f\\n\\r\\t\"").ShouldSucceed("\b\f\n\r\t");
			JsonParsers.JsonString.TryParse("\"\\u0000\\ufFfF\"").ShouldSucceed("\u0000\ufFfF");
		}

		[Fact]
		public void JsonBooleanSuccess()
		{
			JsonParsers.JsonBoolean.TryParse("true").ShouldSucceed(true, 4);
			JsonParsers.JsonBoolean.TryParse("false").ShouldSucceed(false, 5);
		}

		[Fact]
		public void JsonNullSuccess()
		{
			JsonParsers.JsonNull.TryParse("null").ShouldSucceed((object) null, 4);
		}

		[Fact]
		public void JsonArraySuccess()
		{
			JsonParsers.JsonBoolean.JsonArrayOf().TryParse("[]").ShouldSucceed(new bool[0]);
			JsonParsers.JsonBoolean.JsonArrayOf().TryParse("[true]").ShouldSucceed(new[] { true });
			JsonParsers.JsonBoolean.JsonArrayOf().TryParse("[ false, false ]").ShouldSucceed(new[] { false, false });
			JsonParsers.JsonNumber.Or(JsonParsers.JsonNull).JsonArrayOf().TryParse("[ 2, null, -3.14 ]").ShouldSucceed(new object[] { 2L, null, -3.14 });
		}

		[Fact]
		public void JsonObjectSuccess()
		{
			JsonParsers.JsonBoolean.JsonPropertyOf().JsonObjectOf().TryParse("{}").ShouldSucceed(new KeyValuePair<string, bool>[0]);
			JsonParsers.JsonBoolean.JsonPropertyOf().JsonObjectOf().TryParse("{\r\n\t\"yes\": true,\r\n\t\"no\": false\r\n}")
				.ShouldSucceed(new[] { new KeyValuePair<string, bool>("yes", true), new KeyValuePair<string, bool>("no", false) });
		}

		[Fact]
		public void JsonValueSuccess()
		{
			const string json = @"
				{
					""id"": 42,
					""names"": [
						{ ""first"": ""Alice"", ""last"": ""Wonderland"" },
						{ ""first"": ""Bob"", ""last"": ""Smith"" }
					]
				}";

			var value = (IReadOnlyList<KeyValuePair<string, object>>) JsonParsers.JsonValue.TryParse(json).Value;
			value[0].Key.ShouldBe("id");
			value[0].Value.ShouldBe(42L);
			value[1].Key.ShouldBe("names");
			var names = ((IReadOnlyList<object>) value[1].Value).Cast<IReadOnlyList<KeyValuePair<string, object>>>().ToList();
			names[0][0].Key.ShouldBe("first");
			names[1][1].Value.ShouldBe("Smith");
		}
	}
}
