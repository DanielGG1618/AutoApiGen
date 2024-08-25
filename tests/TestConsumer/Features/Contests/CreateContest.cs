using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TestConsumer.Features.Contests;

public static class CreateContest
{
    [PostEndpoint("contests/{Id:int=56}",
        SuccessCode = Status201Created
    )]
    public record Command(
        string Name,
        bool Status = false,
        int Id = 56,
        decimal Price = 12.42m,
        float Currency = 1.23f
    ) : IRequest<Contest>;

    public class Handler : IRequestHandler<Command, Contest>
    {
        public ValueTask<Contest> Handle(Command request, CancellationToken cancellationToken) =>
            ValueTask.FromResult(new Contest(request.Id, request.Name, request.Status, []));
    }
}