# Parser.AtLeastOnce&lt;T&gt; method

Succeeds if the parser succeeds at least once. The value is a collection of as many items as can be successfully parsed.

```csharp
public static IParser<IReadOnlyList<T>> AtLeastOnce<T>(this IParser<T> parser)
```

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Parsing.dll -->
