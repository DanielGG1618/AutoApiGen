using System.Collections.Immutable;
using AutoApiGen.Extensions;
using AutoApiGen.Models;
using AutoApiGen.Templates;

namespace AutoApiGen;

internal class ControllerDataBuilder(
    ImmutableArray<EndpointContractModel> endpoints,
    string? rootNamespace,
    string mediatorPackageName
)
{
    private readonly ImmutableArray<EndpointContractModel> _endpoints = endpoints;

    private readonly string _controllersNamespace =
        rootNamespace is null
            ? "Controllers"
            : $"{rootNamespace}.Controllers";

    private readonly string _mediatorPackageName = mediatorPackageName;

    private readonly Dictionary<string, ControllerData> _controllers = [];

    public ImmutableArray<ControllerData> Build()
    {
        foreach (var endpoint in _endpoints)
            IncludeRequestFrom(endpoint);

        return _controllers.Values.ToImmutableArray();
    }

    private void IncludeRequestFrom(EndpointContractModel endpoint)
    {
        var routeParameters =
            endpoint.Attribute.Route.GetParameters().Select(ParameterData.FromRoute).ToImmutableArray();

        var request = CreateRequestData(endpoint, routeParameters);
        var method = CreateMethodData(endpoint, routeParameters, request);

        AddRequestToCorrespondingController(endpoint.Attribute.Route.BaseRoute, request, method);
    }

    private void AddRequestToCorrespondingController(string? baseRoute, RequestData? request, MethodData method)
    {
        var controllerName = baseRoute?.WithCapitalFirstLetter() ?? StaticData.EmptyBaseRouteControllerName;

        if (_controllers.TryGetValue(controllerName, out var controller))
        {
            if (request.HasValue)
                controller.Requests.Add(request.Value);
            controller.Methods.Add(method);

            return;
        }

        _controllers[controllerName] = new ControllerData(
            _mediatorPackageName,
            _controllersNamespace,
            baseRoute,
            controllerName,
            [method],
            request.HasValue ? [request.Value] : []
        );
    }

    private static MethodData CreateMethodData(
        EndpointContractModel endpoint,
        ImmutableArray<ParameterData> routeParameters,
        RequestData? request
    ) => new(
        endpoint.Attribute.HttpMethod,
        endpoint.Attribute.Route.GetRelationalRoute(),
        Attributes: "",
        Name: endpoint.RequestName,
        Parameters: routeParameters,
        RequestType: request.HasValue ? $"{request.Value.Name}Request" : null,
        request?.Parameters.Select(p => p.Name).ToImmutableArray(),
        endpoint.ContractTypeFullName,
        [..endpoint.Parameters.Select(p => p.Name)],
        endpoint.ResponseTypeFullName
    );

    private static RequestData? CreateRequestData(
        EndpointContractModel endpoint,
        ImmutableArray<ParameterData> routeParameters
    )
    {
        var routeParametersNames = routeParameters.Select(p => p.Name).ToImmutableHashSet();

        return endpoint.Parameters
                .Where(parameter => !routeParametersNames.Contains(parameter.Name))
                .Select(ParameterData.FromSymbol).ToImmutableArray()
            is { Length: > 0 } parameters
            ? new RequestData(
                endpoint.RequestName,
                parameters
            )
            : null;
    }
}
