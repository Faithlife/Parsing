# Parser class

Helper methods for creating and executing parsers.

```csharp
public static class Parser
```

## Public Members

| name | description |
| --- | --- |
| static [AnyChar](Parser/AnyChar.md) | Parses any character; i.e. only fails at the end of the text. |
| static [Digit](Parser/Digit.md) | Parses any digit (as determined by System.Char.IsDigit). |
| static [Letter](Parser/Letter.md) | Parses any letter (as determined by System.Char.IsLetter). |
| static [LetterOrDigit](Parser/LetterOrDigit.md) | Parses any letter or digit (as determined by System.Char.IsLetterOrDigit). |
| static [WhiteSpace](Parser/WhiteSpace.md) | Parses any whitespace character (as determined by System.Char.IsWhiteSpace). |
| static [AnyCharExcept](Parser/AnyCharExcept.md)(…) | Parses any character except the specified character. |
| static [Append&lt;T&gt;](Parser/Append.md)(…) | Appends a successfully parsed value to the end of a successfully parsed collection. |
| static [AtLeast&lt;T&gt;](Parser/AtLeast.md)(…) | Succeeds if the parser succeeds at least the specified number of times. The value is a collection of as many items as can be successfully parsed. |
| static [AtLeastOnce&lt;T&gt;](Parser/AtLeastOnce.md)(…) | Succeeds if the parser succeeds at least once. The value is a collection of as many items as can be successfully parsed. |
| static [AtMost&lt;T&gt;](Parser/AtMost.md)(…) | Always succeeds. The value is a collection of at most the specified number of successfully parsed items. |
| static [AtMostOnce&lt;T&gt;](Parser/AtMostOnce.md)(…) | Always succeeds. The value is a one-item collection of a single successfully parsed item; otherwise an empty collection. |
| static [Bracketed&lt;T,U,V&gt;](Parser/Bracketed.md)(…) | Succeeds if the specified parsers succeed beforehand and afterward (ignoring their results). |
| static [Char](Parser/Char.md)(…) | Parses a single character if the specified predicate returns true. (2 methods) |
| static [Chars](Parser/Chars.md)(…) | Maps a successfully parsed string into a successfully parsed collection of characters. |
| static [Concat](Parser/Concat.md)(…) | Concatenates the successfully parsed collection of strings into a single successfully parsed string. |
| static [Concat&lt;T&gt;](Parser/Concat.md)(…) | Concatenates the two successfully parsed collections. |
| static [Create&lt;T&gt;](Parser/Create.md)(…) | Creates a parser from a delegate. |
| static [Delimited&lt;T,U&gt;](Parser/Delimited.md)(…) | Succeeds if the specified parser succeeds at least once, requiring and ignoring the specified delimiter between each item. |
| static [DelimitedAllowTrailing&lt;T,U&gt;](Parser/DelimitedAllowTrailing.md)(…) | Succeeds if the specified parser succeeds at least once, requiring and ignoring the specified delimiter between each item, and allowing a single optional trailing delimiter. |
| static [End&lt;T&gt;](Parser/End.md)(…) | Succeeds only at the end of the text. |
| static [FollowedBy&lt;T,U&gt;](Parser/FollowedBy.md)(…) | Succeeds if the specified parser also succeeds afterward (ignoring its result). |
| static [Join](Parser/Join.md)(…) | Joins the successfully parsed collection of strings into a single successfully parsed string using the specified separator. |
| static [Many&lt;T&gt;](Parser/Many.md)(…) | Always succeeds. The value is a collection of as many items as can be successfully parsed. |
| static [Named&lt;T&gt;](Parser/Named.md)(…) | Reports a named failure with the specified name if the parser fails. |
| static [Once&lt;T&gt;](Parser/Once.md)(…) | Succeeds if the parser succeeds. The value is a one-item collection of the successfully parsed item. |
| static [Or&lt;T&gt;](Parser/Or.md)(…) | Succeeds with a successful parser, if any. (3 methods) |
| static [OrDefault&lt;T&gt;](Parser/OrDefault.md)(…) | Succeeds with the default value if the parser fails. (2 methods) |
| static [OrEmpty&lt;T&gt;](Parser/OrEmpty.md)(…) | Succeeds with an empty collection if the parser fails. (3 methods) |
| static [Parse&lt;T&gt;](Parser/Parse.md)(…) | Parses the specified text, throwing ParseException on failure. (2 methods) |
| static [Positioned&lt;T&gt;](Parser/Positioned.md)(…) | Wraps the text position and length around a successfully parsed value. |
| static [PrecededBy&lt;T,U&gt;](Parser/PrecededBy.md)(…) | Succeeds if the specified parser also succeeds beforehand (ignoring its result). |
| static [Ref&lt;T&gt;](Parser/Ref.md)(…) | Refers to another parser indirectly. This allows circular compile-time dependency between parsers. |
| static [Regex](Parser/Regex.md)(…) | Succeeds if the specified regular expression pattern matches the text. (2 methods) |
| static [Repeat&lt;T&gt;](Parser/Repeat.md)(…) | Succeeds if the parser succeeds the specified number of times. The value is a collection of the parsed items. (2 methods) |
| static [Select&lt;T,U&gt;](Parser/Select.md)(…) | Converts any successfully parsed value. |
| static [SelectMany&lt;T,U,V&gt;](Parser/SelectMany.md)(…) | Used to support LINQ query syntax. |
| static [String](Parser/String.md)(…) | Parses the specified string using ordinal (case-sensitive) comparison. (3 methods) |
| static [Success&lt;T&gt;](Parser/Success.md)(…) | Succeeds with the specified value without advancing the text position. |
| static [Success&lt;T,U&gt;](Parser/Success.md)(…) | Succeeds with the specified value if the parser is successful. |
| static [Then&lt;T,U&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Trim&lt;T&gt;](Parser/Trim.md)(…) | Succeeds if the specified parser succeeds, ignoring any whitespace characters beforehand or afterward. |
| static [TrimEnd&lt;T&gt;](Parser/TrimEnd.md)(…) | Succeeds if the specified parser succeeds, ignoring any whitespace characters afterward. |
| static [TrimStart&lt;T&gt;](Parser/TrimStart.md)(…) | Succeeds if the specified parser succeeds, ignoring any whitespace characters beforehand. |
| static [TryParse&lt;T&gt;](Parser/TryParse.md)(…) | Attempts to parse the specified text. (2 methods) |
| static [Where&lt;T&gt;](Parser/Where.md)(…) | Fails if the specified predicate returns false for the successfully parsed value. |

## See Also

* namespace [Faithlife.Parsing](../Faithlife.Parsing.md)
* [Parser.cs](https://github.com/Faithlife/Parsing/tree/master/src/Faithlife.Parsing/Parser.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Parsing.dll -->
