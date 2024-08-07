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
    public sealed class RequiredMemberAttribute : Attribute;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    // ReSharper disable once UnusedType.Global
    public sealed class CompilerFeatureRequiredAttribute : Attribute
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
    public sealed class SetsRequiredMembersAttribute : Attribute;
}
