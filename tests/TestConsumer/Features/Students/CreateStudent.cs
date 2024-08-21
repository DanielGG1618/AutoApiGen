using System;
using OneOf;
using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using ErrorOr;
using Mediator;

namespace TestConsumer.Features.Students;

public record Student(string Name);

[PostEndpoint("students")] 
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
        
        var student = new Student(name);

        repo.Add(student);

        return student;
    }
}
