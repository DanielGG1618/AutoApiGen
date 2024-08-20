using System.Collections.Generic;

namespace TestConsumer.Features.Contests;

public record Contest(int Id, string Name, bool Status, List<Participant> Participants);
public record Participant(int Id, string Name);