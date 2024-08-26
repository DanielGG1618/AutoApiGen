using System.Collections.Immutable;
using AutoApiGen.Extensions;
using AutoApiGen.Generators;
using AutoApiGen.Models;
using AutoApiGen.Templates;
using PostInitParameter = AutoApiGen.Templates.ToActionResultMethodTemplate.ParameterData.PostInit;
using LiteralParameter = AutoApiGen.Templates.ToActionResultMethodTemplate.ParameterData.Literal;

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

    private readonly Lazy<ResponseKind.ResultType> _resultTypeResponse = new(() =>
        resultType is null ? throw new ArgumentException("Result type configuration is not set")
            : new ResponseKind.ResultType(
                ToActionResultMethod: default,
                resultType.MatchMethodName,
                resultType.ErrorHandlerMethod?.Name
                ?? throw new ArgumentException("Error handler method is not set")
            )
    );

    private const string EmptyBaseRouteControllerName = "Root";

    private readonly Dictionary<string, ControllerTemplate.Data> _controllers = [];

    public ImmutableArray<ControllerTemplate.Data> Build()
    {
        foreach (var endpoint in _endpoints)
            IncludeRequestFrom(endpoint);

        PostProcessDatas();

        return _controllers.Values.ToImmutableArray();
    }

    private void PostProcessDatas()
    {
        foreach (var controller in _controllers.Values)
        {
            string? getActionName = null;
            List<ResponseKind.NonVoid>? responsesToModify = null;

            foreach (var method in controller.Methods)
                switch (method)
                {
                    case { HttpMethod: "Get", Parameters: [{ Name: "id" }] }:
                        getActionName = method.Name;
                        break;
                    case { HttpMethod: "Post", ResponseKind: ResponseKind.NonVoid response }:
                        if (getActionName is null)
                        {
                            responsesToModify ??= new List<ResponseKind.NonVoid>();
                            responsesToModify.Add(response);
                            break;
                        }

                        PostProcessPostMethodData(response, controller.Name, getActionName);
                        break;
                }

            if (responsesToModify is null)
                continue;

            if (getActionName is null)
                throw new InvalidOperationException("No Get Action found for CreatedAtAction response");

            foreach (var response in responsesToModify)
                PostProcessPostMethodData(response, controller.Name, getActionName);
        }
        return;

        static void PostProcessPostMethodData(
            ResponseKind.NonVoid response,
            string controllerName,
            string getMethodName
        )
        {
            for (var i = 0; i < response.ToActionResultMethod.ExternalParameters.Length; i++)
                response.ToActionResultMethod.ExternalParameters[i] =
                    response.ToActionResultMethod.ExternalParameters[i] switch
                    {
                        PostInitParameter("ControllerName") => new LiteralParameter(controllerName),
                        PostInitParameter("GetActionName") => new LiteralParameter(getMethodName),
                        _ => response.ToActionResultMethod.ExternalParameters[i] // Do nothing
                    };
        }
    }

    private void IncludeRequestFrom(EndpointContractModel endpoint)
    {
        var request = CreateRequestData(
            endpoint,
            routeParameterNames: endpoint.Attribute.Route.Parameters.Select(p => p.Name).ToImmutableHashSet()
        );

        var method = CreateMethodData(
            endpoint,
            endpoint.Attribute.Route.Parameters.Select(ParameterTemplate.Data.FromRoute).ToImmutableArray(),
            request
        );

        AddRequestToCorrespondingController(endpoint.Attribute.Route.BaseRoute, request, method);
    }

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

    private MethodTemplate.Data CreateMethodData(
        EndpointContractModel endpoint,
        ImmutableArray<ParameterTemplate.Data> routeParameters,
        RequestTemplate.Data? request
    ) => new(
        endpoint.Attribute.HttpMethod,
        endpoint.Attribute.Route.RelationalRoute,
        Attributes(endpoint.ResponseType, endpoint.Attribute.SuccessCode, endpoint.Attribute.ErrorCodes),
        Name: endpoint.RequestName,
        Parameters: routeParameters,
        RequestType: request.HasValue ? $"{request.Value.Name}Request" : null,
        request?.Parameters.Select(p => p.Name).ToImmutableArray(),
        endpoint.ContractTypeFullName,
        endpoint.Parameters.Select(p => p.Name).ToImmutableArray(),
        ResponseKindFor(endpoint.ResponseType?.Name, endpoint.Attribute.SuccessCode)
    );

    private string Attributes(
        TypeModel? responseType,
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

    private ResponseKind ResponseKindFor(string? responseTypeName, int successCode) =>
        responseTypeName switch
        {
            null => ResponseKind.Void.Instance,

            _ when responseTypeName == _resultTypeName => _resultTypeResponse.Value with
            {
                ToActionResultMethod = ToActionResultMethodTemplate.For(successCode)
            },

            _ => new ResponseKind.RawNonVoid(ToActionResultMethodTemplate.For(successCode))
        };

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
            _mediatorPackageName
        );
    }
}
