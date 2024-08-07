using AutoApiGen.Attributes;
using MediatR;

namespace AspNetConsumer.Features.Contests;

public static class GetContest
{
    [GetEndpoint("contests/{id}")]
    public record Query(int Id) : IRequest<Contest>;
    
    public class Handler : IRequestHandler<Query, Contest>
    {
        public Task<Contest> Handle(Query query, CancellationToken cancellationToken)
        {
            var id = query.Id;

            return Task.FromResult(new Contest(id, "Contest"));
        }
    }
}
