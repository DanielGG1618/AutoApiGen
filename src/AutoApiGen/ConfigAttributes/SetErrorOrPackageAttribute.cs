#pragma warning disable CS9113 // Parameter is unread.

namespace AutoApiGen.ConfigAttributes;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class SetErrorOrPackageAttribute(string errorOrPackage) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
