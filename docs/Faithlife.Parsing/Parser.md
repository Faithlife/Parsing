# Parser class

Helper methods for creating and executing parsers.

```csharp
public static class Parser
```

## Public Members

| name | description |
| --- | --- |
| static readonly [AnyChar](Parser/AnyChar.md) | Parses any character; i.e. only fails at the end of the text. |
| static readonly [Digit](Parser/Digit.md) | Parses any digit (as determined by Char)). |
| static readonly [Letter](Parser/Letter.md) | Parses any letter (as determined by Char)). |
| static readonly [LetterOrDigit](Parser/LetterOrDigit.md) | Parses any letter or digit (as determined by Char)). |
| static readonly [WhiteSpace](Parser/WhiteSpace.md) | Parses any whitespace character (as determined by Char)). |
| static [AnyCharExcept](Parser/AnyCharExcept.md)(…) | Parses any character except the specified character. |
| static [Append&lt;T&gt;](Parser/Append.md)(…) | Appends a successfully parsed value to the end of a successfully parsed collection. |
| static [AtLeast&lt;T&gt;](Parser/AtLeast.md)(…) | Succeeds if the parser succeeds at least the specified number of times. The value is a collection of as many items as can be successfully parsed. |
| static [AtLeastOnce&lt;T&gt;](Parser/AtLeastOnce.md)(…) | Succeeds if the parser succeeds at least once. The value is a collection of as many items as can be successfully parsed. |
| static [AtMost&lt;T&gt;](Parser/AtMost.md)(…) | Always succeeds. The value is a collection of at most the specified number of successfully parsed items. |
| static [AtMostOnce&lt;T&gt;](Parser/AtMostOnce.md)(…) | Always succeeds. The value is a one-item collection of a single successfully parsed item; otherwise an empty collection. |
| static [Bracketed&lt;TValue,TBracketing&gt;](Parser/Bracketed.md)(…) | Succeeds if the specified parser succeeds beforehand and afterward (ignoring its results). |
| static [Bracketed&lt;TValue,TPreceding,TFollowing&gt;](Parser/Bracketed.md)(…) | Succeeds if the specified parsers succeed beforehand and afterward (ignoring their results). |
| static [Capture&lt;T&gt;](Parser/Capture.md)(…) | Captures the parsed text as a string. |
| static [ChainBinary&lt;TValue,TOperator&gt;](Parser/ChainBinary.md)(…) | Chains a left-associative binary operator to the parser. |
| static [ChainUnary&lt;TValue,TOperator&gt;](Parser/ChainUnary.md)(…) | Chains a left-associative unary operator to the parser. |
| static [Char](Parser/Char.md)(…) | Parses a single character if the specified predicate returns true. (2 methods) |
| static [Chars](Parser/Chars.md)(…) | Maps a successfully parsed string into a successfully parsed collection of characters. |
| static [Concat](Parser/Concat.md)(…) | Concatenates the successfully parsed collection of strings into a single successfully parsed string. |
| static [Concat&lt;T&gt;](Parser/Concat.md)(…) | Concatenates the two successfully parsed collections. |
| static [Create&lt;T&gt;](Parser/Create.md)(…) | Creates a parser from a delegate. |
| static [Delimited&lt;TValue,TDelimiter&gt;](Parser/Delimited.md)(…) | Succeeds if the specified parser succeeds at least once, requiring and ignoring the specified delimiter between each item. |
| static [DelimitedAllowTrailing&lt;TValue,TDelimiter&gt;](Parser/DelimitedAllowTrailing.md)(…) | Succeeds if the specified parser succeeds at least once, requiring and ignoring the specified delimiter between each item, and allowing a single optional trailing delimiter. |
| static [End&lt;T&gt;](Parser/End.md)(…) | Succeeds only at the end of the text. |
| static [Failure&lt;T&gt;](Parser/Failure.md)() | Succeeds with the specified value without advancing the text position. |
| static [Failure&lt;T&gt;](Parser/Failure.md)(…) | Fails even if the parser is successful. |
| static [FollowedBy&lt;TValue,TFollowing&gt;](Parser/FollowedBy.md)(…) | Succeeds if the specified parser also succeeds afterward (ignoring its result). |
| static [Join](Parser/Join.md)(…) | Joins the successfully parsed collection of strings into a single successfully parsed string using the specified separator. |
| static [Many&lt;T&gt;](Parser/Many.md)(…) | Always succeeds. The value is a collection of as many items as can be successfully parsed. |
| static [Named&lt;T&gt;](Parser/Named.md)(…) | Reports a named failure with the specified name if the parser fails. |
| static [Not&lt;T&gt;](Parser/Not.md)(…) | Fails if the parser succeeds, and succeeds with the default value if it fails. |
| static [Once&lt;T&gt;](Parser/Once.md)(…) | Succeeds if the parser succeeds. The value is a one-item collection of the successfully parsed item. |
| static [Or&lt;T&gt;](Parser/Or.md)(…) | Succeeds with a successful parser, if any. (3 methods) |
| static [OrDefault&lt;T&gt;](Parser/OrDefault.md)(…) | Succeeds with the default value if the parser fails. (2 methods) |
| static [OrEmpty&lt;T&gt;](Parser/OrEmpty.md)(…) | Succeeds with an empty collection if the parser fails. (3 methods) |
| static [Parse&lt;T&gt;](Parser/Parse.md)(…) | Parses the specified text, throwing [`ParseException`](./ParseException.md) on failure. (2 methods) |
| static [Positioned&lt;T&gt;](Parser/Positioned.md)(…) | Wraps the text position and length around a successfully parsed value. |
| static [PrecededBy&lt;TValue,TPreceding&gt;](Parser/PrecededBy.md)(…) | Succeeds if the specified parser also succeeds beforehand (ignoring its result). |
| static [Ref&lt;T&gt;](Parser/Ref.md)(…) | Refers to another parser indirectly. This allows circular compile-time dependency between parsers. |
| static [Regex](Parser/Regex.md)(…) | Succeeds if the specified regular expression pattern matches the text. (2 methods) |
| static [Repeat&lt;T&gt;](Parser/Repeat.md)(…) | Succeeds if the parser succeeds the specified number of times. The value is a collection of the parsed items. (2 methods) |
| static [Select&lt;TBefore,TAfter&gt;](Parser/Select.md)(…) | Converts any successfully parsed value. |
| static [SelectMany&lt;TBefore,TDuring,TAfter&gt;](Parser/SelectMany.md)(…) | Used to support LINQ query syntax. |
| static [SkipThen&lt;T1,T2&gt;](Parser/SkipThen.md)(…) | Executes one parser after another, ignoring the output of the first parser. |
| static [String](Parser/String.md)(…) | Parses the specified string using ordinal (case-sensitive) comparison. (3 methods) |
| static [Success&lt;T&gt;](Parser/Success.md)(…) | Succeeds with the specified value without advancing the text position. |
| static [Success&lt;TBefore,TAfter&gt;](Parser/Success.md)(…) | Succeeds with the specified value if the parser is successful. |
| static [Then&lt;T1,T2&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Then&lt;TBefore,TAfter&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Then&lt;T1,T2,T3&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Then&lt;T1,T2,TAfter&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Then&lt;T1,T2,T3,T4&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Then&lt;T1,T2,T3,T4,T5&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Then&lt;T1,T2,T3,T4,T5,T6&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Then&lt;T1,T2,T3,T4,T5,T6,T7&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [Then&lt;T1,T2,T3,T4,T5,T6,T7,T8&gt;](Parser/Then.md)(…) | Executes one parser after another. |
| static [ThenSkip&lt;T1,T2&gt;](Parser/ThenSkip.md)(…) | Executes one parser after another, ignoring the output of the second parser. |
| static [Trim&lt;T&gt;](Parser/Trim.md)(…) | Succeeds if the specified parser succeeds, ignoring any whitespace characters beforehand or afterward. |
| static [TrimEnd&lt;T&gt;](Parser/TrimEnd.md)(…) | Succeeds if the specified parser succeeds, ignoring any whitespace characters afterward. |
| static [TrimStart&lt;T&gt;](Parser/TrimStart.md)(…) | Succeeds if the specified parser succeeds, ignoring any whitespace characters beforehand. |
| static [TryParse&lt;T&gt;](Parser/TryParse.md)(…) | Attempts to parse the specified text. (4 methods) |
| static [Where&lt;T&gt;](Parser/Where.md)(…) | Fails if the specified predicate returns false for the successfully parsed value. |

## See Also

* namespace [Faithlife.Parsing](../Faithlife.Parsing.md)
* [Parser.cs](https://github.com/Faithlife/Parsing/tree/master/src/Faithlife.Parsing/Parser.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Parsing.dll -->
