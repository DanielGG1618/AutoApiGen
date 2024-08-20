#pragma warning disable CS9113 // Parameter is unread.

namespace AutoApiGen;

// ReSharper disable once RedundantAttributeUsageProperty
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class SetErrorOrPackageAttribute(string errorOrPackage) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
