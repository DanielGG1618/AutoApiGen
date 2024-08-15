using AutoApiGen.Attributes;
using MediatR;

namespace TestConsumer.Features.Students;

public record Student(string Name);

[PostEndpoint("students")] 
public record CreateStudentCommand(string Name) : IRequest<Student>;

public class CreateStudentHandler(StudentsRepo repo) : IRequestHandler<CreateStudentCommand, Student>
{
    public Task<Student> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name;

        var student = new Student(name);
        
        repo.Add(student);
        
        return Task.FromResult(student);
    }
}
