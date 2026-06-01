#if !NET
using System.Runtime.CompilerServices;

namespace Faithlife.Parsing;

internal static class ExceptionExtensions
{
	extension(ArgumentNullException)
	{
		public static void ThrowIfNull(object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
		{
			if (argument is null)
				throw new ArgumentNullException(paramName);
		}
	}
}
#endif
