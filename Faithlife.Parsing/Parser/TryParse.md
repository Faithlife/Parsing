# Parser.TryParse&lt;T&gt; method (1 of 4)

Attempts to parse the specified text.

```csharp
public static IParseResult<T> TryParse<T>(this IParser<T> parser, string text)
```

## See Also

* interface [IParseResult&lt;T&gt;](../IParseResult-1.md)
* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

---

# Parser.TryParse&lt;T&gt; method (2 of 4)

Attempts to parse the specified text at the specified start index.

```csharp
public static IParseResult<T> TryParse<T>(this IParser<T> parser, string text, int startIndex)
```

## See Also

* interface [IParseResult&lt;T&gt;](../IParseResult-1.md)
* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

---

# Parser.TryParse&lt;T&gt; method (3 of 4)

Attempts to parse the specified text.

```csharp
public static bool TryParse<T>(this IParser<T> parser, string text, out T value)
```

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

---

# Parser.TryParse&lt;T&gt; method (4 of 4)

Attempts to parse the specified text at the specified start index.

```csharp
public static bool TryParse<T>(this IParser<T> parser, string text, int startIndex, out T value)
```

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Parsing.dll -->