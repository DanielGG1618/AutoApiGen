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
            global::ErrorOr.ErrorType.Conflict =>
                global::Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict,
            global::ErrorOr.ErrorType.Validation =>
                global::Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest,
            global::ErrorOr.ErrorType.NotFound =>
                global::Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound,
            _ => global::Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError
        },
        title: errors[0].Description
    );
    
    private global::Microsoft.AspNetCore.Mvc.IActionResult Problem(
        global::ErrorOr.Error error
    ) => Problem(
        statusCode: error.Type switch
        {
            global::ErrorOr.ErrorType.Conflict =>
                global::Microsoft.AspNetCore.Http.StatusCodes.Status409Conflict,
            global::ErrorOr.ErrorType.Validation =>
                global::Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest,
            global::ErrorOr.ErrorType.NotFound =>
                global::Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound,
            global::ErrorOr.ErrorType.Forbidden =>
                global::Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden,
            _ => global::Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError
        },
        title: error.Description
    );
    """
)]