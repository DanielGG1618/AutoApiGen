## Given
```csharp
[PostEndpoint("/students")]
public record CreateStudentCommand(string Name) : ICommand<Student>;

public class CreateStudentHandler : ICommandHandler<CreateStudentCommand, Student>
{
    public async Task<Student> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name;
        
        return new Student(name);
    }
}

[GetEndpoint("/students/{Id}")]
public record GetStudentQuery(string Id) : IQuery<Student>;

public class GetStudentHandler : IQueryHandler<GetStudentQuery, Student>
{
    public async Task<Student> Handle(GetStudentQuery query, CancellationToken cancellationToken)
    {
        var id = query.Id;
        
        return new Student(id);
    }
}
```

## Generates
```csharp
[Route("students")]
public class StudentsController(IMediator mediator) 
    : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        var command = new CreateStudentCommand(request.Name);
        var result = await Mediator.Send(command);
        
        return Created(result);
        //return CreatedAtAction("GetStudent", "Students", result.Id, result);
    }
    
    [HttpGet("{id:string}")]
    public async Task<IActionResult> GetStudent(string id)
    {
        var query = new GetStudentQuery(id);
        var result = await Mediator.Send(query);
        
        return Ok(result);
    }
}
```

## For Future Thought 
- Generating custom types in case of mix of route, query, and body parameters