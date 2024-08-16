using System.Collections.Immutable;
using AutoApiGen.DataObjects;
using AutoApiGen.Extensions;
using AutoApiGen.Wrappers;

namespace AutoApiGen;

internal class ControllerDataBuilder(
    ImmutableArray<EndpointContractDeclarationSyntax> endpoints,
    string? rootNamespace,
    string mediatorPackageName
)
{
    private readonly ImmutableArray<EndpointContractDeclarationSyntax> _endpoints = endpoints;

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

    private void IncludeRequestFrom(EndpointContractDeclarationSyntax endpoint)
    {
        var routeParameters = endpoint.GetRouteParameters().Select(ParameterData.FromRoute).ToImmutableArray();
        var requestName = endpoint.GetRequestName();

        var request = CreateRequestData(endpoint, routeParameters, requestName);
        var method = CreateMethodData(endpoint, routeParameters, request);

        AddRequestToCorrespondingController(endpoint.BaseRoute, request, method);
    }

    private void AddRequestToCorrespondingController(string baseRoute, RequestData? request, MethodData method)
    {
        var controllerName = baseRoute.WithCapitalFirstLetter();

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
        EndpointContractDeclarationSyntax endpoint,
        ImmutableArray<ParameterData> routeParameters,
        RequestData? request
    ) => new(
        endpoint.GetHttpMethod(),
        endpoint.GetRelationalRoute(),
        Attributes: "",
        Name: request?.Name ?? endpoint.GetRequestName(),
        Parameters: routeParameters,
        RequestType: request.HasValue ? $"{request.Value.Name}Request" : null,
        request?.Parameters.Select(p => p.Name).ToImmutableArray(),
        endpoint.GetContractType(),
        [..endpoint.GetParameters().Select(p => p.Name())],
        endpoint.GetResponseType()
    );

    private static RequestData? CreateRequestData(
        EndpointContractDeclarationSyntax endpoint,
        ImmutableArray<ParameterData> routeParameters,
        string requestName
    )
    {
        var routeParametersNames = routeParameters.Select(p => p.Name).ToImmutableHashSet();

        return endpoint.GetParameters()
                .Where(parameter => !routeParametersNames.Contains(parameter.Name()))
                .Select(ParameterData.FromSyntax).ToImmutableArray()
            is { Length: > 0 } parameters
            ? new RequestData(
                requestName,
                parameters
            )
            : null;
    }
}
