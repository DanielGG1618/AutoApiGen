using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    // ReSharper disable once UnusedType.Global
    internal static class IsExternalInit;
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    // ReSharper disable once UnusedType.Global
    internal sealed class RequiredMemberAttribute : Attribute;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    // ReSharper disable once UnusedType.Global
    internal  sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string FeatureName { get; }

        // ReSharper disable once UnusedMember.Global
        public bool IsOptional { get; init; }
 
        public CompilerFeatureRequiredAttribute(string featureName) 
            => FeatureName = featureName;
    }
}

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    // ReSharper disable once UnusedType.Global
    internal  sealed class SetsRequiredMembersAttribute : Attribute;
}

#nullable disable
namespace System
{
    internal readonly struct Range
    {
        public Index Start { get; }
        public Index End { get; }

        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        public static Range StartAt(Index start) => new(start, Index.End);

        public static Range EndAt(Index end) => new(Index.Start, end);

        public static Range All => new(Index.Start, Index.End);

        public override string ToString() => $"{Start}..{End}";
    }

    internal readonly struct Index
    {
        private readonly int _value;

        public Index(int value, bool fromEnd = false) => 
            _value = fromEnd ? ~value : value;

        public static Index Start => new(0);
        public static Index End => new(~0);

        public int Value => _value < 0 ? ~_value : _value;

        public bool IsFromEnd => _value < 0;

        public override string ToString() => IsFromEnd ? $"^{Value}" : Value.ToString();

        public int GetOffset(int length) =>
            length < 0 ? throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.")
            : IsFromEnd ? length - Value : Value;

        public static implicit operator Index(int value) => new(value);

        public override bool Equals(object obj) => obj is Index index && _value == index._value;
        public override int GetHashCode() => _value;
    }
}