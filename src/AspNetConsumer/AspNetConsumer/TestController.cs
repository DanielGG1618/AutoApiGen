using Microsoft.AspNetCore.Mvc;

namespace AspNetConsumer;

// {<name>(:<type>)(?|=<default>)}
// {*<name>(:<type>)(=<default>)}
// bool IsOptional <-> bool IsCatchAll
// (there are only 3 options => false false, true false, false true)
// btw if it is optional it cannot have default value
// string? Type
// string? Default
// Syntax that is "valid" in ASP .NET Core, but is not in AutoApiGen
//  - "/{:route:int?}/
// Btw you can use as many semicolons to define type as you want
//  - But not in my project
// Also I don't support optional parameters in path
// But I do support default values

// Regex for route in AutoApiGen:
// @"([a-zA-Z_]\w*)(:[a-zA-Z_]\w*)?(\?|=.+)?"
// @"(\*[a-zA-Z_]\w*)(:[a-zA-Z_]\w*)?(=.+)?)"

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("get/{:route:int}/{id:Guid}")]
    public IActionResult Get(int? route, Guid id) => Ok($"{route}: {id}");
}
