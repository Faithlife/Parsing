#if NETSTANDARD2_0
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
	internal sealed class AllowNullAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
	internal sealed class MaybeNullAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
	internal sealed class NotNullAttribute : Attribute
	{
	}
}

#endif
