using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;

namespace TestConsumer.Features.Contests;

public static class UpdateContest
{
    [PutEndpoint("contests/{Id:int}")]
    public record Command(int Id, string Name) : ICommand<Contest>;
    
    public class Handler : ICommandHandler<Command, Contest>
    {
        public ValueTask<Contest> Handle(Command request, CancellationToken cancellationToken) => 
            ValueTask.FromResult(new Contest(request.Id, request.Name));
    }
}
