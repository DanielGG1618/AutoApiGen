using AutoApiGen.Attributes;
using MediatR;

namespace AspNetConsumer.Features.Contests;

public static class CreateContest
{
    [PostEndpoint("contests/{Id:int=52}")]
    public record Command(string Name, int Id = 52) : IRequest<Contest>;
    
    public class Handler : IRequestHandler<Command, Contest>
    {
        public Task<Contest> Handle(Command request, CancellationToken cancellationToken) => 
            Task.FromResult(new Contest(request.Id, request.Name));
    }
}
