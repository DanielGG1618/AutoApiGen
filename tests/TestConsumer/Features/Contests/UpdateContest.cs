using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;

namespace TestConsumer.Features.Contests;

public static class UpdateContest
{
    [PutEndpoint("contests/{Id:int}")]
    public record Command(int Id, string Name, List<Participant> Participants) : ICommand;
    
    public class Handler : ICommandHandler<Command>
    {
        public ValueTask<Unit> Handle(Command request, CancellationToken cancellationToken) => 
            Unit.ValueTask;
    }
}
