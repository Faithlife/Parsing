# Parser.Regex method (1 of 3)

Succeeds if the specified regular expression matches the text.

```csharp
public static IParser<Match> Regex(Regex regex)
```

## Remarks

The regular expression should be anchored at the beginning of the text (via `^`).

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

---

# Parser.Regex method (2 of 3)

Succeeds if the specified regular expression pattern matches the text.

```csharp
public static IParser<Match> Regex(string pattern)
```

## Remarks

The regular expression pattern is automatically anchored at the beginning of the text, but not at the end of the text. The parsed value is the successful Match.

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

---

# Parser.Regex method (3 of 3)

Succeeds if the specified regular expression pattern matches the text.

```csharp
public static IParser<Match> Regex(string pattern, RegexOptions regexOptions)
```

## Remarks

The regular expression pattern is automatically anchored at the beginning of the text, but not at the end of the text. The parsed value is the successful Match.

## See Also

* interface [IParser&lt;T&gt;](../IParser-1.md)
* class [Parser](../Parser.md)
* namespace [Faithlife.Parsing](../../Faithlife.Parsing.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Parsing.dll -->