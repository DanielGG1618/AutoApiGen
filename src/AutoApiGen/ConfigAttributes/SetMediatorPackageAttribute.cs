﻿#pragma warning disable CS9113 // Parameter is unread.

namespace AutoApiGen.ConfigAttributes;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class SetMediatorPackageAttribute(string mediatorPackage) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
