# Faithlife.Parsing

A simple library for constructing parsers in C#.

**Faithlife.Parsing** is inspired by, adapted from, and owes its existence to the [Sprache](https://github.com/sprache/Sprache) C# library. Be sure to consult the [Sprache documentation](https://github.com/sprache/Sprache/blob/master/README.md) for more information about the overall strategy used by Faithlife.Parsing, as well as various examples, tutorials, and acknowledgements. Particular thanks go to [Nicholas Blumhardt](http://nblumhardt.com), the author of Sprache.

Faithlife.Parsing allows you to define complex parsers in just a few lines of C#. Ease of parser creation is the only advantage of Faithlife.Parsing over more robust parsing libraries. This library is not fast, not efficient, probably not safe for potentially hostile user input, and does not have great error reporting. Use at your own risk.

The Faithlife.Parsing library includes [parsers for JSON](src/Faithlife.Parsing/Json/JsonParsers.cs). Reading this code is a good way to learn how complex parsers are constructed, but you should definitely prefer specialized JSON parsers for production work.
