using Microsoft.CodeAnalysis;

namespace AutoApiGen.Generators;

[Generator]
internal sealed class ConfigAttributesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput(c =>
            c.AddSource("ConfigAttributes.g.cs", $$"""
            {{StaticData.GeneratedDisclaimer}}
            
            #nullable enable
            
            namespace AutoApiGen.ConfigAttributes;
            
            [global::System.AttributeUsage(System.AttributeTargets.Assembly)]
            public sealed class ResultTypeConfigurationAttribute 
                : global::System.Attribute
            {
                public required string TypeName { get; init; }
                public required string MatchMethodName { get; init; } = "Match";
                public string? ErrorHandlerMethodName { get; init; }
                public string? ErrorHandlerMethodImplementation { get; init; }
            }
            
            #pragma warning disable CS9113 // Parameter is unread.
            [global::System.AttributeUsage(System.AttributeTargets.Assembly)]
            public sealed class SetMediatorPackageAttribute(string mediatorPackage) 
                : global::System.Attribute;
            #pragma warning restore CS9113 // Parameter is unread.
            """)
        );
}

