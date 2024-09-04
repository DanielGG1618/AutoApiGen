## Given
```csharp
[PostEndpoint("/students",
    SuccessCode = StatusCodes.Status201Created,
)]
public record CreateStudentCommand(string Name) : ICommand<Student>;

public class CreateStudentHandler : ICommandHandler<CreateStudentCommand, Student>
{
    public async Task<Student> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name;
        
        return new Student(name);
    }
}

[GetEndpoint("/students/{Id}",
    ErrorCode = StatusCode.Status404NotFound
)]
public record GetStudentQuery(string Id) : IQuery<Student>;

public class GetStudentHandler : IQueryHandler<GetStudentQuery, Student>
{
    public async Task<Student?> Handle(GetStudentQuery query, CancellationToken cancellationToken)
    {
        var id = query.Id;
        
        if (id is 0)
        {
            //somehow return NotFound or throw exception
        }
        
        return new Student(id);
    }
}

[PutEndpoint("/students/file")]
public record UpdateStudentsFromFileCommand([FromForm] IFormFile File) : ICommand;

public class UpdateStudentsFromFileHandler : ICommandHandler<UpdateStudentsFromFileCommand>
{
    public async Task<Unit> Handle(UpdateStudentsFromFileCommand command, CancellationToken cancellationToken)
    {
        var file = command.File;
        
        ...
    }        
}
```

## Generates
```csharp
record CreateStudentRequest(string Name); 

[Route("students")]
public class StudentsController(IMediator mediator)
{
    private readonly IMediator _mediator = mediator;
    
    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        var command = new CreateStudentCommand(request.Name);
        var result = await _mediator.Send(command);
        
        return Created(result);
        //return CreatedAtAction("GetStudent", "Students", result.Id, result);
    }
    
    [HttpGet("{id:string}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetStudent(string id)
    {
        var query = new GetStudentQuery(id);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
    
    [HttpPut("file")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateStudentsFromFile(IFormFile file)
    {
        var command = new UpdateStudentsFromFileCommand(
            File: file
        );
        
        await _mediator.Send(query);
        
        return NoContent();
    }
}
```
