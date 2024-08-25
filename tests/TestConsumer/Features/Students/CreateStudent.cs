using System;
using OneOf;
using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace TestConsumer.Features.Students;

public record Student(string Name, string Id);

[PostEndpoint("students",
    SuccessCode = StatusCodes.Status201Created,
    ErrorCode = StatusCodes.Status403Forbidden
)] 
public record CreateStudentCommand(string Name) : ICommand<OneOf<Student, Error>>;

public class CreateStudentHandler(StudentsRepo repo) 
    : ICommandHandler<CreateStudentCommand, OneOf<Student, Error>>
{
    public async ValueTask<OneOf<Student, Error>> Handle(
        CreateStudentCommand command,
        CancellationToken cancellationToken
    )
    {
        await ValueTask.CompletedTask;

        var name = command.Name;

        if (name.Equals("Badam", StringComparison.OrdinalIgnoreCase))
            return Error.Forbidden();
        
        var student = new Student(name, Random.Shared.Next().ToString());

        repo.Add(student);

        return student;
    }
}
