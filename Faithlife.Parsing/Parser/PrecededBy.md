# Parser.PrecededBy&lt;TValue,TPreceding&gt; method

Succeeds if the specified parser also succeeds beforehand (ignoring its result).

```csharp
public static IParser<TValue> PrecededBy<TValue, TPreceding>(this IParser<TValue> parser, 
    IParser<TPreceding> precededBy)
```

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Parsing.dll -->