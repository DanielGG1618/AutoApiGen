using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;

namespace TestConsumer.Features.Contests;

public static class CreateContest
{
    [PostEndpoint("contests/{Id:int=52}")]
    public record Command(string Name, bool Status, int Id = 52) : IRequest<Contest>;
    
    public class Handler : IRequestHandler<Command, Contest>
    {
        public ValueTask<Contest> Handle(Command request, CancellationToken cancellationToken) => 
            ValueTask.FromResult(new Contest(request.Id, request.Name, request.Status, []));
    }
}