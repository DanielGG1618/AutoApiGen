using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TestConsumer.Controllers;

public sealed partial class ContestsController
{
    public record NewResponse(int Lenght, string Text);
    
    [HttpGet("test")]
    [ProducesResponseType<NewResponse>(Status200OK)]
    [ProducesResponseType<ProblemDetails>(Status404NotFound)]
    public IActionResult TestEndpoint([FromQuery] string? text) =>
        text is null ? NotFound() : Ok(new NewResponse(text.Length, text));
}
