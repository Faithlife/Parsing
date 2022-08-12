# Release Notes

## 3.3.0

* Support unary/binary operators with less recursion.

## 3.2.0

* Add maximum depth support to `Ref`.

## 3.1.0

* Add standalone `End`.

## 3.0.0

* **Breaking:** Add new method to `IParser<T>` that improves performance of chained parsers.
  * Upgrading to version 3 should be safe except when `IParser<T>` has been implemented directly. Custom parsers should now derive from `Parser<T>`.
* **Breaking:** Default to culture invariant regular expressions.
* Add `bool`-returning `TryParse` overloads.
* Add optimized `Then` overloads for building tuples.
* Add `Capture`, `Not`, and `Failure`.
* Add single-parameter `Bracketed`.
* Miscellaneous optimizations.

## 2.3.0

* Add `Then` overloads with better performance.
* A failing `Or` should return the farthest failure position.
* Miscellaneous optimizations.
* Improve nullability of `GetValueOrDefault` for C# 10.

## 2.2.0

* Support left-associative unary and binary operators.

## 2.1.0

* Use nullable references.
* Fix analyzer issues.

## 2.0.0

* Drop support for .NET Standard 1.1 and .NET 4.6.1.

## 1.2.1

* Update Source Link to 2.8.0.

## 1.2.0

* Adopt [Faithlife/CSharpTemplate](https://github.com/Faithlife/CSharpTemplate).
