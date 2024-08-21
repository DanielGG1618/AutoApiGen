using Microsoft.CodeAnalysis;

namespace AutoApiGen.Generators;

[Generator]
internal sealed class EndpointAttributesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput(c =>
            c.AddSource("EndpointAttributes.g.cs", $"""
                {StaticData.GeneratedDisclaimer}

                namespace AutoApiGen.Attributes;

                #pragma warning disable CS9113 // Parameter is unread.
                [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true)]
                internal abstract class EndpointAttribute(
                    [global::System.Diagnostics.CodeAnalysis.StringSyntax("Route")] string route
                ) : global::System.Attribute;
                #pragma warning restore CS9113 // Parameter is unread.

                internal sealed class GetEndpointAttribute(
                    [global::System.Diagnostics.CodeAnalysis.StringSyntax("Route")] string route = ""
                ) : global::AutoApiGen.Attributes.EndpointAttribute(route);
                internal sealed class PostEndpointAttribute(
                    [global::System.Diagnostics.CodeAnalysis.StringSyntax("Route")] string route = ""
                ) : global::AutoApiGen.Attributes.EndpointAttribute(route);
                internal sealed class PutEndpointAttribute(
                    [global::System.Diagnostics.CodeAnalysis.StringSyntax("Route")] string route = ""
                ) : global::AutoApiGen.Attributes.EndpointAttribute(route);
                internal sealed class DeleteEndpointAttribute(
                    [global::System.Diagnostics.CodeAnalysis.StringSyntax("Route")] string route = ""
                ) : global::AutoApiGen.Attributes.EndpointAttribute(route);
                internal sealed class HeadEndpointAttribute(
                    [global::System.Diagnostics.CodeAnalysis.StringSyntax("Route")] string route = ""
                ) : global::AutoApiGen.Attributes.EndpointAttribute(route);
                internal sealed class PatchEndpointAttribute(
                    [global::System.Diagnostics.CodeAnalysis.StringSyntax("Route")] string route = ""
                ) : global::AutoApiGen.Attributes.EndpointAttribute(route);
                internal sealed class OptionsEndpointAttribute(
                    [global::System.Diagnostics.CodeAnalysis.StringSyntax("Route")] string route = ""
                ) : global::AutoApiGen.Attributes.EndpointAttribute(route);
                """
            )
        );
}
