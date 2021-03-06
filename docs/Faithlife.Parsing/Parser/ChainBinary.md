# Parser.ChainBinary&lt;TValue,TOperator&gt; method

Chains a left-associative binary operator to the parser.

```csharp
public static IParser<TValue> ChainBinary<TValue, TOperator>(this IParser<TValue> parser, 
    IParser<TOperator> opParser, Func<TOperator, TValue, TValue, TValue> apply)
```

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Parsing.dll -->
