using System.CodeDom.Compiler;
using AutoApiGen.Exceptions;
using AutoApiGen.Extensions;

namespace AutoApiGen.Templates;

internal abstract record ResponseReturnTemplate
{
    public sealed record Void : ResponseReturnTemplate
    {
        public static Void Instance { get; } = new();
        private Void() {}
    }

    public sealed record RawNonVoid(in ToActionResultMethodTemplate ToActionResultMethod) : ResponseReturnTemplate;

    public sealed record ResultType(
        in ToActionResultMethodTemplate ToActionResultMethod,
        string MatchMethodName,
        string ErrorHandlerMethodName
    ) : ResponseReturnTemplate;

    public void RenderTo(IndentedTextWriter writer) => writer.WriteLines(
        this switch
        {
            Void =>
                """
                await _mediator.Send(contract, cancellationToken);

                return NoContent();
                """,

            RawNonVoid(var toActionResult) =>
                $"""
                var result = await _mediator.Send(contract, cancellationToken);

                return {toActionResult.Render("result")};
                """,

            ResultType(var toActionResult, string match, string onError) =>
                $"""
                var result = await _mediator.Send(contract, cancellationToken);

                return result.{match}(
                    x => {toActionResult.Render("x")},
                    errors => {onError}(errors)
                );
                """,

            _ => throw new ThisIsUnionException(nameof(ResponseReturnTemplate))
        }
    );

    private ResponseReturnTemplate() {}
}
