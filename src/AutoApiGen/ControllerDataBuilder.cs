using System.Collections.Immutable;
using AutoApiGen.Extensions;
using AutoApiGen.Templates;
using AutoApiGen.Wrappers;

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
        var routeParameters = endpoint.Attribute.GetRouteParameters().Select(ParameterData.FromRoute).ToImmutableArray();
        var requestName = endpoint.RequestTypeFullName;

        var request = CreateRequestData(endpoint, routeParameters, requestName);
        var method = CreateMethodData(endpoint, routeParameters, request);

        AddRequestToCorrespondingController(endpoint.Attribute.BaseRoute, request, method);
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
        endpoint.Attribute.GetHttpMethod(),
        endpoint.Attribute.GetRelationalRoute(),
        Attributes: "",
        Name: request?.Name ?? endpoint.RequestTypeFullName,
        Parameters: routeParameters,
        RequestType: request.HasValue ? $"{request.Value.Name}Request" : null,
        request?.Parameters.Select(p => p.Name).ToImmutableArray(),
        endpoint.ContractTypeFullName,
        [..endpoint.Parameters.Select(p => p.Name())],
        endpoint.ResponseTypeFullName
    );

    private static RequestData? CreateRequestData(
        EndpointContractModel endpoint,
        ImmutableArray<ParameterData> routeParameters,
        string requestName
    )
    {
        var routeParametersNames = routeParameters.Select(p => p.Name).ToImmutableHashSet();

        return endpoint.Parameters
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
