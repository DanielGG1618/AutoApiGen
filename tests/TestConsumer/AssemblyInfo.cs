[assembly: AutoApiGen.ConfigAttributes.SetMediatorPackage("global::Mediator")]
[assembly: AutoApiGen.ConfigAttributes.ResultTypeConfiguration(
    TypeName = "OneOf",//"ErrorOr"
    MatchMethodName = "Match",
    ErrorHandlerMethodName = "Problem",
    ErrorHandlerMethodImplementation = """
    private global::Microsoft.AspNetCore.Mvc.IActionResult Problem(
        global::System.Collections.Generic.List<global::ErrorOr.Error> errors
    ) => Problem(
        statusCode: errors[0].Type switch
        {
            global::ErrorOr.ErrorType.Conflict => 409,
            global::ErrorOr.ErrorType.Validation => 400,
            global::ErrorOr.ErrorType.NotFound => 404,
            global::ErrorOr.ErrorType.Forbidden => 403,
            _ => 500
        },
        title: errors[0].Description
    );
    
    private global::Microsoft.AspNetCore.Mvc.IActionResult Problem(
        global::ErrorOr.Error error
    ) => Problem(
        statusCode: error.Type switch
        {
            global::ErrorOr.ErrorType.Conflict => 409,
            global::ErrorOr.ErrorType.Validation => 400,
            global::ErrorOr.ErrorType.NotFound => 404,
            global::ErrorOr.ErrorType.Forbidden => 403,
            _ => 500
        },
        title: error.Description
    );
    """
)]