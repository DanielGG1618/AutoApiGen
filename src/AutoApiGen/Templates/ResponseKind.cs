namespace AutoApiGen.Templates;

internal abstract record ResponseKind
{
    public sealed record Void : ResponseKind
    {
        public static Void Instance { get; } = new();
        private Void() {}
    }

    public record NonVoid(ToActionResultMethodTemplate.Data ToActionResultMethod) : ResponseKind;

    public sealed record RawNonVoid(ToActionResultMethodTemplate.Data ToActionResultMethod)
        : NonVoid(ToActionResultMethod);

    public sealed record ResultType(
        ToActionResultMethodTemplate.Data ToActionResultMethod,
        string MatchMethodName,
        string ErrorHandlerMethodName
    ) : NonVoid(ToActionResultMethod);

    private ResponseKind() {}
}
