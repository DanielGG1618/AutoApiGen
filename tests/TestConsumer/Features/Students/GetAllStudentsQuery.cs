using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;

namespace TestConsumer.Features.Students;

public static class GetAllStudentsQuery
{
    [GetEndpoint("students")]
    public record Query : IQuery<List<Student>>;

    public class Handler(StudentsRepo repo) : IQueryHandler<Query, List<Student>>
    {
        public ValueTask<List<Student>> Handle(Query query, CancellationToken cancellationToken) =>
            ValueTask.FromResult(repo.GetAll());
    }
}
