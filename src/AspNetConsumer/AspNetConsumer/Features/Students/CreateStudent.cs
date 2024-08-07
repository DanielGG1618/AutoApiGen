using AutoApiGen.Attributes;
using MediatR;

namespace AspNetConsumer.Features.Students;

public record Student(string Name);

[PostEndpoint("students")] 
public record CreateStudentCommand(string Name) : IRequest<Student>;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Student>
{
    public Task<Student> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name;

        return Task.FromResult(new Student(name));
    }
}
