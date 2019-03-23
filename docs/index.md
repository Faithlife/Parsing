# Faithlife.Parsing

**Faithlife.Parsing** is a simple library for constructing parsers in C#.

## Overview

This library is inspired by, adapted from, and owes its existence to the [Sprache](https://github.com/sprache/Sprache) C# library. Be sure to consult the [Sprache documentation](https://github.com/sprache/Sprache/blob/master/README.md) for more information about the overall strategy used by Faithlife.Parsing, as well as various examples, tutorials, and acknowledgements. Particular thanks go to [Nicholas Blumhardt](http://nblumhardt.com), the author of Sprache.

Faithlife.Parsing allows you to define complex parsers in just a few lines of C#. Ease of parser creation is the only advantage of Faithlife.Parsing over more robust parsing libraries. This library is not fast, not efficient, probably not safe for potentially hostile user input, and does not have great error reporting. Use at your own risk.

The Faithlife.Parsing library includes [parsers for JSON](Faithlife.Parsing.Json/JsonParsers.md). Reading this code is a good way to learn how complex parsers are constructed, but you should definitely prefer specialized JSON parsers for production work.

## Installation

Faithlife.Parsing should be installed [via NuGet](https://www.nuget.org/packages/Faithlife.Parsing).

This library is compatible with most .NET platforms via [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/articles/standard/library).

## Using a parser

A parser is an implementation of [IParser&lt;T&gt;](Faithlife.Parsing/IParser-1.md). It converts one or more characters of text into an instance of type `T`.

```csharp
// parses one or more digits into an integer
IParser<int> integerParser = CreateIntegerParser();
```

To use a parser, you call [TryParse](Faithlife.Parsing/Parser/TryParse.md) or [Parse](Faithlife.Parsing/Parser/Parse.md), extension methods for [IParser&lt;T&gt;](Faithlife.Parsing/IParser-1.md) that are defined on the [Parser](Faithlife.Parsing/Parser.md) static class. These methods have a `text` parameter that specifies the text that you want to parse.

[TryParse](Faithlife.Parsing/Parser/TryParse.md) returns an [IParseResult&lt;T&gt;](Faithlife.Parsing/IParseResult-1.md). The [Success](Faithlife.Parsing/IParseResult/Success.md) property is true if the parsing was successful, in which case the [Value](Faithlife.Parsing/IParseResult-1/Value.md) property returns the result of the parse.

```csharp
IParseResult<int> integerResult = integerParser.TryParse("123abc");
Debug.Assert(integerResult.Success);
Debug.Assert(integerResult.Value == 123);
```

If [Success](Faithlife.Parsing/IParseResult/Success.md) is false, the parsing failed. Accessing the [Value](Faithlife.Parsing/IParseResult-1/Value.md) property will throw a [ParseException](Faithlife.Parsing/ParseException.md).

```csharp
IParseResult<int> noIntegerResult = integerParser.TryParse("abc123");
Debug.Assert(!noIntegerResult.Success);
```

[Parse](Faithlife.Parsing/Parser/Parse.md) returns the successfully parsed value of type `T` directly.

```csharp
int number = integerParser.Parse("123abc");
Debug.Assert(number == 123);
```

If the parsing fails, [Parse](Faithlife.Parsing/Parser/Parse.md) throws a [ParseException](Faithlife.Parsing/ParseException.md), which has a [Result](Faithlife.Parsing/ParseException/Result.md) property that contains the full [IParseResult&lt;T&gt;](Faithlife.Parsing/IParseResult-1.md).

```csharp
try
{
    int noNumber = integerParser.Parse("abc123");
}
catch (ParseException exception)
{
    Debug.Assert(!exception.Result.Success);
}
```

[IParseResult&lt;T&gt;](Faithlife.Parsing/IParseResult-1.md) also indicates where parsing stopped. The [NextPosition](Faithlife.Parsing/IParseResult/NextPosition.md) property of [IParseResult&lt;T&gt;](Faithlife.Parsing/IParseResult-1.md) returns a [TextPosition](Faithlife.Parsing/TextPosition.md) whose [Index](Faithlife.Parsing/TextPosition/Index.md) property indicates the index into the text where parsing stopped.

```csharp
Debug.Assert(integerResult.NextPosition.Index == 3);
```

When parsing fails, the next position is always where the parse started.

```csharp
Debug.Assert(noIntegerResult.NextPosition.Index == 0);
```

## Character parsers

Use [Parser.Char](Faithlife.Parsing/Parser/Char.md) to parse a character that matches the specified predicate.

```csharp
IParser<char> digitParser = Parser.Char(ch => ch >= '0' && ch <= '9');
Debug.Assert(digitParser.Parse("789xyz") == '7');
Debug.Assert(!digitParser.TryParse("xyz789").Success);
```

Other character parsers include [AnyChar](Faithlife.Parsing/Parser/AnyChar.md), [AnyCharExcept](Faithlife.Parsing/Parser/AnyCharExcept.md), [Digit](Faithlife.Parsing/Parser/Digit.md), [Letter](Faithlife.Parsing/Parser/Letter.md), [LetterOrDigit](Faithlife.Parsing/Parser/LetterOrDigit.md), and [WhiteSpace](Faithlife.Parsing/Parser/WhiteSpace.md).

## String parsers

Use [Parser.String](Faithlife.Parsing/Parser/String.md) to match one or more consecutive characters.

```csharp
IParser<string> yesParser = Parser.String("yes");
Debug.Assert(yesParser.Parse("yesno") == "yes");
Debug.Assert(!yesParser.TryParse("noyes").Success);
```

Specify `StringComparison.OrdinalIgnoreCase` if desired.

```csharp
IParser<string> noParser = Parser.String("no", StringComparison.OrdinalIgnoreCase);
Debug.Assert(noParser.Parse("No way") == "No");
```

## Repeating parsers

Use [AtLeastOnce](Faithlife.Parsing/Parser/AtLeastOnce.md) to allow a parser to be used one or more times consecutively.

```csharp
IParser<IReadOnlyList<char>> digitsParser = digitParser.AtLeastOnce();
Debug.Assert(digitsParser.Parse("789xyz").SequenceEqual(new[] { '7', '8', '9' }));
```

To specify the number of times that a parser can be repeated, use one of the many repeat parsers: [Many](Faithlife.Parsing/Parser/Many.md) (0..∞), [AtMost](Faithlife.Parsing/Parser/AtMost.md) (0..n), [AtMostOnce](Faithlife.Parsing/Parser/AtMostOnce.md) (0..1), [AtLeastOnce](Faithlife.Parsing/Parser/AtLeastOnce.md) (1..∞), [AtLeast](Faithlife.Parsing/Parser/AtLeast.md) (n..∞), [Once](Faithlife.Parsing/Parser/Once.md) (1), and [Repeat](Faithlife.Parsing/Parser/Repeat.md) (n or n..m).

Use [Delimited](Faithlife.Parsing/Parser/Delimited.md) to repeat a parser at least once but also require a delimiter between the parsed items. ([DelimitedAllowTrailing](Faithlife.Parsing/Parser/DelimitedAllowTrailing.md) allows an optional delimiter at the end.)

```csharp
IReadOnlyList<char> letters = Parser.Letter.Delimited(Parser.Char(',')).Parse("a,b,c");
Debug.Assert(letters.SequenceEqual(new[] { 'a', 'b', 'c' }));
```

## Converting parsers

The real power of Faithlife.Parsing is the ability to create parsers that also interpret the text. The familiar [Select](Faithlife.Parsing/Parser/Select.md) extension method can be used to convert a successfully parsed value into something more useful.

The following parser (used above) matches a digit, converts the digit to an integer, repeats that digit-to-integer parser at least once, and then converts those integers-from-digits into the final integer that the digits represent together.

```csharp
IParser<int> CreateIntegerParser()
{
    return Parser.Char(ch => ch >= '0' && ch <= '9')
        .Select(ch => (int)ch - '0')
        .AtLeastOnce()
        .Select(digits => digits.Aggregate(0, (x, y) => x * 10 + y));
}
```

Use [Success](Faithlife.Parsing/Parser/Success.md) to return a specific value for any successful parsing.

```csharp
IParser<bool> falseParser = Parser.String("false", StringComparison.OrdinalIgnoreCase).Success(false);
Debug.Assert(falseParser.Parse("False") == false);
```

There are also built-in parsers for converting lists of characters and strings.

* [Chars](Faithlife.Parsing/Parser/Chars.md) – Converts a string parser to a character-list parser.
* [String](Faithlife.Parsing/Parser/String.md) – Converts a character-list parser to a string parser.
* [Concat](Faithlife.Parsing/Parser/Concat.md) – Converts a string-list parser into a string parser by concatenating the strings.
* [Join](Faithlife.Parsing/Parser/Join.md) – Converts a string-list parser into a string parser by joining the strings with a separator.

```csharp
IParser<string> keywordsParser = Parser.Letter.AtLeastOnce().String()
    .Delimited(Parser.WhiteSpace.AtLeastOnce()).Join(",");
Debug.Assert(keywordsParser.Parse("public  static readonly") == "public,static,readonly");
```

## Chaining parsers

The [SelectMany](Faithlife.Parsing/Parser/SelectMany.md) extension method allows LINQ query syntax to be used to chain parsers, one after another, and combine the results of each parsing.

```csharp
IParser<int> simpleAdder =
    from left in integerParser
    from plus in Parser.Char('+')
    from right in integerParser
    select left + right;
Debug.Assert(simpleAdder.Parse("7+8") == 15);
```

Other extension methods are useful for chaining parsers when the result of one of them is not used.

* [PrecededBy](Faithlife.Parsing/Parser/PrecededBy.md) – Requires that a parser succeed beforehand.
* [FollowedBy](Faithlife.Parsing/Parser/FollowedBy.md) – Requires that a parser succeed afterward.
* [Bracketed](Faithlife.Parsing/Parser/Bracketed.md) – Calls [PrecededBy](Faithlife.Parsing/Parser/PrecededBy.md) and [FollowedBy](Faithlife.Parsing/Parser/FollowedBy.md).
* [TrimStart](Faithlife.Parsing/Parser/TrimStart.md) – Ignores whitespace beforehand, if any.
* [TrimEnd](Faithlife.Parsing/Parser/TrimEnd.md) – Ignores whitespace afterward, if any.
* [Trim](Faithlife.Parsing/Parser/Trim.md) – Ignores whitespace beforehand or afterward, if any.

```csharp
IParser<IReadOnlyList<int>> tupleParser =
    integerParser.Trim().Delimited(Parser.Char(',')).Bracketed(Parser.Char('('), Parser.Char(')')).Trim();
Debug.Assert(tupleParser.Parse(" (7, 8, 9) ").SequenceEqual(new[] { 7, 8, 9 }));
```

## Either-or parsers

Powerful parsers need to provide either-or options. The [Or](Faithlife.Parsing/Parser/Or.md) methods are used to allow one of any number of available parsers to be used. [Or](Faithlife.Parsing/Parser/Or.md) can be used as a static method or as an extension method.

```csharp
IParser<bool> yesNoParser = yesParser.Success(true).Or(noParser.Success(false));
Debug.Assert(yesNoParser.Parse("yes") == true);
Debug.Assert(yesNoParser.Parse("no") == false);

IParser<bool?> yesNoMaybeParser = Parser.Or(
    yesParser.Success((bool?) true),
    noParser.Success((bool?) false),
    Parser.String("maybe").Success((bool?) null));
Debug.Assert(yesNoMaybeParser.Parse("yes") == true);
Debug.Assert(yesNoMaybeParser.Parse("no") == false);
Debug.Assert(yesNoMaybeParser.Parse("maybe") == null);
```

## Filtering parsers

You can restrict the values that can be produced by a parser by using [Where](Faithlife.Parsing/Parser/Where.md) (or `where` in LINQ query syntax).

```csharp
IParser<int> positiveParser = integerParser.Where(x => x > 0);
Debug.Assert(positiveParser.Parse("1") == 1);
Debug.Assert(!positiveParser.TryParse("0").Success);
```

## Regular expressions

Use [Parser.Regex](Faithlife.Parsing/Parser/Regex.md) to create a parser that matches the specified regular expression. The regular expression is automatically anchored to start where the text is being parsed.

Regular expressions in .NET are extremely powerful; use them to simplify your parsers whenever possible.

```csharp
IParser<double> numberParser = Parser
    .Regex(@"-?(0|[1-9][0-9]*)(.[0-9]+)?([eE][-+]?[0-9]+)?")
    .Select(x => double.Parse(x.ToString(), CultureInfo.InvariantCulture));
Debug.Assert(numberParser.Parse("6.0221409e+23") == 6.0221409e+23);
```

## Syntax error reporting

To improve syntax errors, give parsers a name with the [Named](Faithlife.Parsing/Parser/Named.md) extension method.

```csharp
IParser<double> namedNumberParser = numberParser.Named("number");
```

## Semantic error reporting

For reporting semantic errors, track where parsed values were found with the [Positioned](Faithlife.Parsing/Parser/Positioned.md) extension method, which wraps the parsed value in a [Positioned&lt;T&gt;](Faithlife.Parsing/Positioned-1.md).

* The [Value](Faithlife.Parsing/Positioned-1/Value.md) property of a [Positioned&lt;T&gt;](Faithlife.Parsing/Positioned-1.md) is the parsed value.
* The [Position](Faithlife.Parsing/Positioned-1/Position.md) property of a [Positioned&lt;T&gt;](Faithlife.Parsing/Positioned-1.md) is a [TextPosition](Faithlife.Parsing/TextPosition.md), which has a zero-based [Index](Faithlife.Parsing/TextPosition/Index.md) into the text, as well as a [GetLineColumn](Faithlife.Parsing/TextPosition/GetLineColumn.md) method, which returns a [LineColumn](Faithlife.Parsing/LineColumn.md) value with a one-based [LineNumber](Faithlife.Parsing/LineColumn/LineNumber.md) and [ColumnNumber](Faithlife.Parsing/LineColumn/ColumnNumber.md).
* The [Length](Faithlife.Parsing/Positioned-1/Length.md) property of a [Positioned&lt;T&gt;](Faithlife.Parsing/Positioned-1.md) indicates the text length of the parsed value.

```csharp
IParser<Positioned<double>> positionedNumberParser = numberParser.Positioned().Trim();
Positioned<double> positionedNumber = positionedNumberParser.Parse("\n\t 3.14");
Debug.Assert(positionedNumber.Value == 3.14);
Debug.Assert(positionedNumber.Position.Index == 3);
Debug.Assert(positionedNumber.Position.GetLineColumn().LineNumber == 2);
Debug.Assert(positionedNumber.Position.GetLineColumn().ColumnNumber == 3);
Debug.Assert(positionedNumber.Length == 4);
```

## Low-level parsers

As stated earlier, a parser is an implementation of [IParser&lt;T&gt;](Faithlife.Parsing/IParser-1.md). Most parsers are created by using the existing parsers documented above, but it is possible to create a custom parser by implementing [IParser&lt;T&gt;](Faithlife.Parsing/IParser-1.md).

[IParser&lt;T&gt;](Faithlife.Parsing/IParser-1.md) has a single method named [TryParse](Faithlife.Parsing/IParser-1/TryParse.md). It takes a single argument of type [TextPosition](Faithlife.Parsing/TextPosition.md), whose [Text](Faithlife.Parsing/TextPosition/Text.md) is the text being parsed and whose [Index](Faithlife.Parsing/TextPosition/Index.md) is the zero-based index into the text where the parsing should begin. [TryParse](Faithlife.Parsing/IParser-1/TryParse.md) returns an [IParseResult&lt;T&gt;](Faithlife.Parsing/IParseResult-1.md), as documented earlier.

You can create a class that implements [IParser&lt;T&gt;](Faithlife.Parsing/IParser-1.md), but it is usually easier to call [Parser.Create](Faithlife.Parsing/Parser/Create.md), which implements [TryParse](Faithlife.Parsing/IParser-1/TryParse.md) with the specified `Func<TextPosition, IParserResult<T>>`.

Your parser should investigate the text, starting at the index specified by the [Index](Faithlife.Parsing/TextPosition/Index.md) property of the [TextPosition](Faithlife.Parsing/TextPosition.md), and determine if it can successfully parse that text.

If it can, the parser should return a successful [IParseResult&lt;T&gt;](Faithlife.Parsing/IParseResult-1.md) via the [ParseResult.Success](Faithlife.Parsing/ParseResult/Success.md) method. Call it with the corresponding value of type `T` and the position just past the end of the text that was parsed. Use the [WithNextIndex](Faithlife.Parsing/TextPosition/WithNextIndex.md) method on the [TextPosition](Faithlife.Parsing/TextPosition.md) to create a text position at the desired index.

If the parser fails, it should return a failed [IParseResult&lt;T&gt;](Faithlife.Parsing/IParseResult-1.md) via the [ParseResult.Failure](Faithlife.Parsing/ParseResult/Failure.md) method.
