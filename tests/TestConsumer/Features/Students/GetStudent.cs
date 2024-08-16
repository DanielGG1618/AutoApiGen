using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;

namespace TestConsumer.Features.Students;

[GetEndpoint("students/{Name}")] 
public record GetStudentQuery(string Name) : IQuery<Student?>;

public class GetStudentHandler(StudentsRepo repo) : IQueryHandler<GetStudentQuery, Student?>
{
    public ValueTask<Student?> Handle(GetStudentQuery query, CancellationToken cancellationToken)
    {
        var name = query.Name;

        return ValueTask.FromResult(repo.Get(name));
    }
}
