using System.Text;
using AutoApiGen.DataObjects;
using Scriban.Runtime;

namespace AutoApiGen.TemplatesProcessing;

internal class ScribanFunctions : ScriptObject
{
    public static string? GetParameter(IEnumerable<ParameterData> parameters, string requestTypeName)
    {
        parameters = parameters as ParameterData[] ?? parameters.ToArray();
        
        return parameters.Any()
            ? GetRequestParameter(parameters, requestTypeName)
            : $"new {requestTypeName}()";
    }

    private static string? GetRequestParameter(IEnumerable<ParameterData> parameters, string requestType)
        => parameters.FirstOrDefault(parameter =>
            parameter.Type.Equals(requestType, StringComparison.CurrentCultureIgnoreCase)
        )?.Name;

    public static string PostInitiate(
        string request,
        IEnumerable<ParameterData> parameters,
        List<string> requestProperties
    )
    {
        parameters = parameters as ParameterData[] ?? parameters.ToArray();
        var additionalParameters = parameters/*.Where(p => p.CanPostInitiateCommand)*/.ToList();

        if (!additionalParameters.Any())
            return string.Empty;

        var stringBuilder = new StringBuilder();
        foreach (var parameter in additionalParameters)
        {
            var property = requestProperties.First(property =>
                property.Equals(parameter.Name, StringComparison.CurrentCultureIgnoreCase)
            );
            stringBuilder.AppendLine($"{GetRequestParameter(parameters, request)}.{property} = {parameter.Name};");
        }

        return stringBuilder.ToString();
    }
}