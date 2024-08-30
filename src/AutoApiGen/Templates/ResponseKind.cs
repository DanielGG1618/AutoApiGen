namespace AutoApiGen.Templates;

internal abstract record ResponseKind
{
    public sealed record Void : ResponseKind
    {
        public static Void Instance { get; } = new();
        private Void() {}
    }

    public sealed record RawNonVoid(in ToActionResultMethodTemplate.Data ToActionResultMethod) : ResponseKind;
    public sealed record ResultType(
        in ToActionResultMethodTemplate.Data ToActionResultMethod,
        string MatchMethodName,
        string ErrorHandlerMethodName
    ) : ResponseKind;

    private ResponseKind() {}
}
