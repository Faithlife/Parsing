# Faithlife.Parsing

**Faithlife.Parsing** is a simple library for constructing parsers in C#.

## Overview

This library is inspired by, adapted from, and owes its existence to the [Sprache](https://github.com/sprache/Sprache) C# library. Be sure to consult the [Sprache documentation](https://github.com/sprache/Sprache/blob/master/README.md) for more information about the overall strategy used by Faithlife.Parsing, as well as various examples, tutorials, and acknowledgements. Particular thanks go to [Nicholas Blumhardt](http://nblumhardt.com), the author of Sprache.

Faithlife.Parsing allows you to define complex parsers in just a few lines of C#. Ease of parser creation is the only advantage of Faithlife.Parsing over more robust parsing libraries. This library is not fast, not efficient, probably not safe for potentially hostile user input, and does not have great error reporting. Use at your own risk.

The Faithlife.Parsing library includes [parsers for JSON](Faithlife.Parsing.Json/JsonParsers.md). Reading this code is a good way to learn how complex parsers are constructed, but you should definitely prefer specialized JSON parsers for production work.

## Installation

Faithlife.Parsing should be installed [via NuGet](https://www.nuget.org/packages/Faithlife.Parsing).

This library is compatible with .NET Framework and .NET Core. Specficially, it is a Portable Class Library using Profile111, which means it supports .NET Framework 4.5, Windows 8, Windows Phone 8.1, and [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/articles/standard/library).

## Using a parser

A parser is an implementation of [IParser<T>](Faithlife.Parsing/IParser-1.md). It converts one or more characters of text into an instance of type `T`.

```csharp
// parses one or more digits into an integer
IParser<int> integerParser = CreateIntegerParser();
```

To use a parser, you call [TryParse](Faithlife.Parsing/Parser/TryParse.md) or [Parse](Faithlife.Parsing/Parser/Parse.md), extension methods for [IParser<T>](Faithlife.Parsing/IParser-1.md) that are defined on the [Parser](Faithlife.Parsing/Parser.md) static class. These methods have a `text` parameter that specifies the text that you want to parse.

[TryParse](Faithlife.Parsing/Parser/TryParse.md) returns an [IParseResult<T>](Faithlife.Parsing/IParseResult-1.md). The [Success](Faithlife.Parsing/IParseResult/Success.md) property is true if the parsing was successful, in which case the [Value](Faithlife.Parsing/IParseResult-1/Value.md) property returns the result of the parse.

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

If the parsing fails, [Parse](Faithlife.Parsing/Parser/Parse.md) throws a [ParseException](Faithlife.Parsing/ParseException.md), which has a [Result](Faithlife.Parsing/ParseException/Result.md) property that contains the full [IParseResult<T>](Faithlife.Parsing/IParseResult-1.md).

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

[IParseResult<T>](Faithlife.Parsing/IParseResult-1.md) also indicates where parsing stopped. The [NextPosition](Faithlife.Parsing/IParseResult/NextPosition.md) property of [IParseResult<T>](Faithlife.Parsing/IParseResult-1.md) returns a [TextPosition](Faithlife.Parsing/TextPosition.md) whose [Index](Faithlife.Parsing/TextPosition/Index.md) property indicates the index into the text where parsing stopped.

```csharp
Debug.Assert(integerResult.NextPosition.Index == 3);
```

When parsing fails, the next position is always where the parse started.

```csharp
Debug.Assert(noIntegerResult.NextPosition.Index == 0);
```

## Character parsers

Use `Parser.Char` to parse a character that matches the specified predicate.

```csharp
IParser<char> digitParser = Parser.Char(ch => ch >= '0' && ch <= '9');
Debug.Assert(digitParser.Parse("789xyz") == '7');
Debug.Assert(!digitParser.TryParse("xyz789").Success);
```

[Other character parsers](Faithlife.Parsing/Parser.md) include `Char(char)`, `AnyChar`, `AnyCharExcept(char)`, `Digit`, `Letter`, `LetterOrDigit`, and `WhiteSpace`.

## String parsers

Use [`Parser.String`](Faithlife.Parsing/Parser.md) to match one or more consecutive characters.

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

Use `AtLeastOnce()` to allow a parser to be used one or more times consecutively.

```csharp
IParser<IReadOnlyList<char>> digitsParser = digitParser.AtLeastOnce();
Debug.Assert(digitsParser.Parse("789xyz").SequenceEqual(new[] { '7', '8', '9' }));
```

To specify the number of times that a parser can be repeated, use one of the many [repeat parsers](Faithlife.Parsing/Parser.md): `Many` (0..∞), `AtMost` (0..n), `AtMostOnce` (0..1), `AtLeastOnce` (1..∞), `AtLeast` (n..∞), `Once` (1), and `Repeat` (n or n..m).

Use `Delimited` to repeat a parser at least once but also require a delimiter between the parsed items. (`DelimitedAllowTrailing` allows an optional delimiter at the end.)

```csharp
IReadOnlyList<char> letters = Parser.Letter.Delimited(Parser.Char(',')).Parse("a,b,c");
Debug.Assert(letters.SequenceEqual(new[] { 'a', 'b', 'c' }));
```

## Converting parsers

The real power of Faithlife.Parsing is the ability to create parsers that also interpret the text. The familiar `Select` extension method can be used to convert a successfully parsed value into something more useful.

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

Use `Success` to return a specific value for any successful parsing.

```csharp
IParser<bool> falseParser = Parser.String("false", StringComparison.OrdinalIgnoreCase).Success(false);
Debug.Assert(falseParser.Parse("False") == false);
```

There are also built-in parsers for converting lists of characters and strings.

* `Chars` – Converts a string parser to a character-list parser.
* `String` – Converts a character-list parser to a string parser.
* `Concat` – Converts a string-list parser into a string parser by concatenating the strings.
* `Join` – Converts a string-list parser into a string parser by joining the strings with a separator.

```csharp
IParser<string> keywordsParser = Parser.Letter.AtLeastOnce().String()
    .Delimited(Parser.WhiteSpace.AtLeastOnce()).Join(",");
Debug.Assert(keywordsParser.Parse("public  static readonly") == "public,static,readonly");
```

## Chaining parsers

The `SelectMany` extension method allows LINQ query syntax to be used to chain parsers, one after another, and combine the results of each parsing.

```csharp
IParser<int> simpleAdder =
    from left in integerParser
    from plus in Parser.Char('+')
    from right in integerParser
    select left + right;
Debug.Assert(simpleAdder.Parse("7+8") == 15);
```

[Other extension methods](Faithlife.Parsing/Parser.md) are useful for chaining parsers when the result of one of them is not used.

* `PrecededBy` – Requires that a parser succeed beforehand.
* `FollowedBy` – Requires that a parser succeed afterward.
* `Bracketed` – Calls `PrecededBy` and `FollowedBy`.
* `TrimStart` – Ignores whitespace beforehand, if any.
* `TrimEnd` – Ignores whitespace afterward, if any.
* `Trim` – Ignores whitespace beforehand or afterward, if any.

```csharp
IParser<IReadOnlyList<int>> tupleParser =
    integerParser.Trim().Delimited(Parser.Char(',')).Bracketed(Parser.Char('('), Parser.Char(')')).Trim();
Debug.Assert(tupleParser.Parse(" (7, 8, 9) ").SequenceEqual(new[] { 7, 8, 9 }));
```

## Either-or parsers

Powerful parsers need to provide either-or options. The [`Or` methods](Faithlife.Parsing/Parser.md) are used to allow one of any number of available parsers to be used. `Or` can be used as a static method or as an extension method.

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

You can restrict the values that can be produced by a parser by using `Where` (or `where` in LINQ query syntax).

```csharp
IParser<int> positiveParser = integerParser.Where(x => x > 0);
Debug.Assert(positiveParser.Parse("1") == 1);
Debug.Assert(!positiveParser.TryParse("0").Success);
```

## Regular expressions

Use `Parser.Regex` to create a parser that matches the specified regular expression. The regular expression is automatically anchored to start where the text is being parsed.

Regular expressions in .NET are extremely powerful; use them to simplify your parsers whenever possible.

```csharp
IParser<double> numberParser = Parser
    .Regex(@"-?(0|[1-9][0-9]*)(.[0-9]+)?([eE][-+]?[0-9]+)?")
    .Select(x => double.Parse(x.ToString(), CultureInfo.InvariantCulture));
Debug.Assert(numberParser.Parse("6.0221409e+23") == 6.0221409e+23);
```

## Syntax error reporting

To improve syntax errors, give parsers a name with the `Named` extension method.

```csharp
IParser<double> namedNumberParser = numberParser.Named("number");
```

## Semantic error reporting

For reporting semantic errors, track where parsed values were found with the `Positioned` extension method, which wraps the parsed value in a `Positioned<T>`.

* The `Value` property of a `Positioned<T>` is the parsed value.
* The `Position` property of a `Positioned<T>` is a `TextPosition`, which has a zero-based `Index` into the text, as well as a `GetLineColumn()` method, which returns a `LineColumn` value with a one-based `LineNumber` and `ColumnNumber`.
* The `Length` property of a `Position<T>` indicates the text length of the parsed value.

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

As stated earlier, a parser is an implementation of `IParser<T>`. Most parsers are created by using the existing parsers documented above, but it is possible to create a custom parser by implementing `IParser<T>`.

`IParser<T>` has a single method named `TryParse`. It takes a single argument of type `TextPosition`, whose `Text` is the text being parsed and whose `Index` is the zero-based index into the text where the parsing should begin. `TryParse` returns an `IParseResult<T>`, as documented earlier.

You can create a class that implements `IParser<T>`, but it is usually easier to call `Parser.Create`, which implements `TryParse` with the specified `Func<TextPosition, IParserResult<T>>`.

Your parser should investigate the text, starting at the index specified by the `Index` property of the `TextPosition`, and determine if it can successfully parse that text.

If it can, the parser should return a successful `IParseResult<T>` via the `ParseResult.Success` method. Call it with the corresponding value of type `T` and the position just past the end of the text that was parsed. Use the `WithNextIndex` method on the `TextPosition` to create a text position at the desired index.

If the parser fails, it should return a failed `IParseResult<T>` via the `ParseResult.Failure` method.
