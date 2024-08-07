using AutoApiGen.Attributes;
using MediatR;

namespace AspNetConsumer.Features.Students;

[GetEndpoint("students/{id}")] 
public record GetStudentQuery(string Id, string Name) : IRequest<Student>;

public class GetStudentHandler : IRequestHandler<GetStudentQuery, Student>
{
    public Task<Student> Handle(GetStudentQuery query, CancellationToken cancellationToken)
    {
        var name = query.Name;

        return Task.FromResult(new Student(name));
    }
}
