using AutoApiGen.Attributes;
using MediatR;

namespace AspNetConsumer.Features.Students;

[GetEndpoint("students/{Name}")] 
public record GetStudentQuery(string Name) : IRequest<Student?>;

public class GetStudentHandler(StudentsRepo repo) : IRequestHandler<GetStudentQuery, Student?>
{
    public Task<Student?> Handle(GetStudentQuery query, CancellationToken cancellationToken)
    {
        var name = query.Name;

        return Task.FromResult(repo.Get(name));
    }
}
