using System;
using System.Collections.Generic;
using Faithlife.Parsing.Json;
using Xunit;

namespace Faithlife.Parsing.Tests.Json
{
	public class JsonDtoTests
	{
		[Fact]
		public void JsonToDto()
		{
			const string json = @"
				{
					""id"": 42,
					""name"": ""widget"",
					""dimensions"": [ 3, 4, 5 ]
				}";

			var widget = WidgetParser.Parse(json);
			widget.Id.ShouldBe(42);
			widget.Name.ShouldBe("widget");
			widget.Dimensions.Count.ShouldBe(3);
			widget.Dimensions[1].ShouldBe(4);
		}

		private sealed class WidgetDto
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public IReadOnlyList<double> Dimensions { get; set; }
		}

		static readonly IParser<Action<WidgetDto>> WidgetIdParser = JsonParsers.JsonInteger.JsonPropertyNamed("id")
			.Select(value => (Action<WidgetDto>) (widget => widget.Id = (int) value));

		static readonly IParser<Action<WidgetDto>> WidgetNameParser = JsonParsers.JsonString.JsonPropertyNamed("name")
			.Select(value => (Action<WidgetDto>) (widget => widget.Name = value));

		static readonly IParser<Action<WidgetDto>> WidgetDimensionsParser = JsonParsers.JsonDouble.JsonArrayOf().JsonPropertyNamed("dimensions")
			.Select(value => (Action<WidgetDto>) (widget => widget.Dimensions = value));

		static readonly IParser<Action<WidgetDto>> WidgetPropertyParser = Parser.Or(WidgetIdParser, WidgetNameParser, WidgetDimensionsParser);

		static readonly IParser<WidgetDto> WidgetParser = WidgetPropertyParser.JsonObjectOf()
			.Select(actions =>
			{
				var widget = new WidgetDto();
				foreach (var action in actions)
					action(widget);
				return widget;
			});
	}
}
