using System;
using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;
using ErrorOr;

namespace TestConsumer.Features.Contests;

public static class GetContestByName
{
    [GetEndpoint("contests/{Name}")]
    public sealed record Query(string Name) : IQuery<ErrorOr<Contest>>;
    
    public sealed class Handler : IQueryHandler<Query, ErrorOr<Contest>>
    {
        public async ValueTask<ErrorOr<Contest>> Handle(Query query, CancellationToken cancellationToken)
        {
            await ValueTask.CompletedTask;
            
            var name = query.Name;

            if (name.Equals("wrong", StringComparison.OrdinalIgnoreCase))
                return Error.Validation();

            return new Contest(52, name, false, []);
        }
    }
}

