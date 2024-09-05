using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using AutoApiGen.Extensions;
using AutoApiGen.Generators;
using AutoApiGen.Models;
using AutoApiGen.Templates;

namespace AutoApiGen;

internal sealed class ControllerTemplateDataBuilder(
    ImmutableArray<EndpointContractModel> endpoints,
    string? rootNamespace,
    string mediatorPackageName,
    ResultTypeConfig? resultType
)
{
    private readonly ImmutableArray<EndpointContractModel> _endpoints = endpoints;

    private readonly string _controllersNamespace =
        rootNamespace is null
            ? "Controllers"
            : $"{rootNamespace}.Controllers";

    private readonly string _mediatorPackageName = mediatorPackageName;

    private readonly string? _resultTypeName = resultType?.TypeName;

    private readonly Lazy<ResponseReturnTemplate.ResultType> _resultTypeResponse = new(() =>
        resultType is null ? throw new ArgumentException("Result type configuration is not set")
            : new ResponseReturnTemplate.ResultType(
                ToActionResultMethod: default,
                resultType.Value.MatchMethodName,
                resultType.Value.ErrorHandlerMethod?.Name
                ?? throw new ArgumentException("Error handler method is not set")
            )
    );

    private const string EmptyBaseRouteControllerName = "Root";

    private readonly Dictionary<string, ControllerTemplate> _controllers = [];

    public ImmutableArray<ControllerTemplate> Build()
    {
        foreach (var endpoint in _endpoints.AsSpan())
            IncludeRequestFrom(endpoint);

        return _controllers.Values.ToImmutableArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void IncludeRequestFrom(in EndpointContractModel endpoint)
    {
        var request = CreateRequestData(endpoint);
        var method = CreateMethodData(endpoint, request);

        AddRequestToCorrespondingController(endpoint.Attribute.Route.BaseRoute, request, method);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    private static RequestTemplate? CreateRequestData(
        in EndpointContractModel endpoint
    ) => endpoint.Parameters
            .Where(parameter => parameter.Source is From.Body)
            .Select(ParameterTemplate.FromModel).ToImmutableArray()
        is not { Length: > 0 } parameters ? null
        : new RequestTemplate(
            endpoint.RequestName,
            parameters
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MethodTemplate CreateMethodData(
        in EndpointContractModel endpoint,
        in RequestTemplate? request
    ) => new(
        endpoint.Attribute.HttpMethod,
        endpoint.Attribute.Route.RelationalRoute,
        Attributes(endpoint.ResponseType, endpoint.Attribute.SuccessCode, endpoint.Attribute.ErrorCodes),
        Name: endpoint.RequestName,
        [..endpoint.Parameters.Where(p => p.Source is not From.Body).Select(ParameterTemplate.FromModel)],
        RequestType: request.HasValue ? $"{request.Value.Name}Request" : null,
        request?.Parameters.Select(p => p.Name).ToImmutableArray(),
        endpoint.ContractTypeFullName,
        endpoint.Parameters.Select(p => p.Name).ToImmutableArray(),
        ResponseReturnTemplateFor(endpoint.ResponseType?.Name, endpoint.Attribute.SuccessCode)
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    private string Attributes(
        in TypeModel? responseType,
        int successCode,
        ImmutableArray<int> errorCodes
    ) => string.Join("\n",
        [
            "[global::Microsoft.AspNetCore.Mvc.ProducesResponseType"
            + (successCode is 204 || responseType is null
                ? "(204)" 
                : $"<{(
                    responseType.Value.Name == _resultTypeName 
                        ? responseType.Value.TypeArguments!.Value[0].FullName
                        : responseType.Value.FullName
                )}>({successCode})")
            + ']',
            ..errorCodes.Select(code =>
                $"[global::Microsoft.AspNetCore.Mvc.ProducesResponseType({code})]"
            )
        ]
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ResponseReturnTemplate ResponseReturnTemplateFor(string? responseTypeName, int successCode) =>
        responseTypeName switch
        {
            null => ResponseReturnTemplate.Void.Instance,

            _ when responseTypeName == _resultTypeName => _resultTypeResponse.Value with
            { ToActionResultMethod = ToActionResultMethodTemplate.For(successCode) },

            _ => new ResponseReturnTemplate.RawNonVoid(ToActionResultMethodTemplate.For(successCode))
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddRequestToCorrespondingController(
        string? baseRoute,
        in RequestTemplate? request,
        in MethodTemplate method
    )
    {
        var controllerName = baseRoute is null or ""
            ? EmptyBaseRouteControllerName
            : baseRoute.WithCapitalFirstLetter();

        if (_controllers.TryGetValue(controllerName, out var controller))
        {
            if (request.HasValue)
                controller.Requests.Add(request.Value);
            controller.Methods.Add(method);

            return;
        }

        _controllers[controllerName] = new ControllerTemplate(
            _controllersNamespace,
            baseRoute,
            controllerName,
            [method],
            request.HasValue ? [request.Value] : [],
            _mediatorPackageName
        );
    }
}
