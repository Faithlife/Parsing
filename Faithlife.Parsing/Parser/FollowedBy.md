# Parser.FollowedBy&lt;TValue,TFollowing&gt; method

Succeeds if the specified parser also succeeds afterward (ignoring its result).

```csharp
public static IParser<TValue> FollowedBy<TValue, TFollowing>(this IParser<TValue> parser, 
    IParser<TFollowing> followedBy)
```

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Parsing.dll -->