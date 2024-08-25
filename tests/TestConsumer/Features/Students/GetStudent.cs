using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;

namespace TestConsumer.Features.Students;

[GetEndpoint("students/{Id}")] 
public record GetStudentQuery(string Id) : IQuery<Student?>;

public class GetStudentHandler(StudentsRepo repo) : IQueryHandler<GetStudentQuery, Student?>
{
    public ValueTask<Student?> Handle(GetStudentQuery query, CancellationToken cancellationToken) => 
        ValueTask.FromResult(repo.Get(query.Id));
}