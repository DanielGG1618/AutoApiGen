namespace AutoApiGen.Exceptions;

internal sealed class ThisIsUnionException(string unionName) 
    : Exception(
        $"{unionName} is a Union type. " +
        $"Waiting for discriminated unions in C# :). " +
        $"Btw if you see this, someone did a terrible thing"
    );