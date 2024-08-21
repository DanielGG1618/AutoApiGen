using System.Collections.Immutable;
using AutoApiGen.Extensions;
using AutoApiGen.Models;
using AutoApiGen.Templates;

namespace AutoApiGen;

internal sealed class ControllerTemplateDataBuilder(
    ImmutableArray<EndpointContractModel> endpoints,
    string? rootNamespace,
    string mediatorPackageName,
    string? errorOrPackageName
)
{
    private readonly ImmutableArray<EndpointContractModel> _endpoints = endpoints;

    private readonly string _controllersNamespace =
        rootNamespace is null
            ? "Controllers"
            : $"{rootNamespace}.Controllers";

    private readonly string _mediatorPackageName = mediatorPackageName;
    private readonly string? _errorOrPackageName = errorOrPackageName;

    private const string EmptyBaseRouteControllerName = "Root";

    private readonly Dictionary<string, ControllerTemplate.Data> _controllers = [];

    public ImmutableArray<ControllerTemplate.Data> Build()
    {
        foreach (var endpoint in _endpoints)
            IncludeRequestFrom(endpoint);

        return _controllers.Values.ToImmutableArray();
    }

    private void IncludeRequestFrom(EndpointContractModel endpoint)
    {
        var request = CreateRequestData(
            endpoint,
            routeParameterNames: endpoint.Attribute.Route.Parameters.Select(p => p.Name).ToImmutableHashSet()
        );

        var method = CreateMethodData(
            endpoint,
            routeParameters: [..endpoint.Attribute.Route.Parameters.Select(ParameterTemplate.Data.FromRoute)],
            request
        );

        AddRequestToCorrespondingController(endpoint.Attribute.Route.BaseRoute, request, method);
    }

    private void AddRequestToCorrespondingController(
        string? baseRoute,
        RequestTemplate.Data? request,
        MethodTemplate.Data method
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

        _controllers[controllerName] = new ControllerTemplate.Data(
            _controllersNamespace,
            baseRoute,
            controllerName,
            [method],
            request.HasValue ? [request.Value] : [],
            _mediatorPackageName,
            _errorOrPackageName
        );
    }

    private static MethodTemplate.Data CreateMethodData(
        EndpointContractModel endpoint,
        ImmutableArray<ParameterTemplate.Data> routeParameters,
        RequestTemplate.Data? request
    ) => new(
        endpoint.Attribute.HttpMethod,
        endpoint.Attribute.Route.RelationalRoute,
        Name: endpoint.RequestName,
        Parameters: routeParameters,
        RequestType: request.HasValue ? $"{request.Value.Name}Request" : null,
        request?.Parameters.Select(p => p.Name).ToImmutableArray(),
        endpoint.ContractTypeFullName,
        [..endpoint.Parameters.Select(p => p.Name)],
        endpoint.ResponseTypeFullName
    );

    private static RequestTemplate.Data? CreateRequestData(
        EndpointContractModel endpoint,
        ISet<string> routeParameterNames
    ) => endpoint.Parameters
            .Where(parameter => !routeParameterNames.Contains(parameter.Name))
            .Select(ParameterTemplate.Data.FromSymbol).ToImmutableArray()
        is { Length: > 0 } parameters
        ? new RequestTemplate.Data(
            endpoint.RequestName,
            parameters
        )
        : null;
}
