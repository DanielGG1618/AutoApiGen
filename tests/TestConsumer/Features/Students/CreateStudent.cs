using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;

namespace TestConsumer.Features.Students;

public record Student(string Name);

[PostEndpoint("students")] 
public record CreateStudentCommand(string Name) : ICommand<Student>;

public class CreateStudentHandler(StudentsRepo repo) : ICommandHandler<CreateStudentCommand, Student>
{
    public ValueTask<Student> Handle(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var name = command.Name;

        var student = new Student(name);
        
        repo.Add(student);
        
        return ValueTask.FromResult(student);
    }
}
