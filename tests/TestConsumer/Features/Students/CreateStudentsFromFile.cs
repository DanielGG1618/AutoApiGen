using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoApiGen.Attributes;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace TestConsumer.Features.Students;

[PostEndpoint("/students/file")]
public record CreateStudentsFromFileCommand(/*[Microsoft.AspNetCore.Mvc.FromForm] both work */IFormFile File) 
    : ICommand;

public class UpdateStudentsFromFileHandler(
    StudentsRepo repo
) : ICommandHandler<CreateStudentsFromFileCommand>
{
    public async ValueTask<Unit> Handle(CreateStudentsFromFileCommand command, CancellationToken cancellationToken)
    {
        await ValueTask.CompletedTask;

        var file = command.File;

        await foreach (var student in ParseStudents(file, cancellationToken))
            repo.Add(student);
        
        return Unit.Value;
    }

    private static async IAsyncEnumerable<Student> ParseStudents(
        IFormFile file,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var content = await new StreamReader(file.OpenReadStream()).ReadToEndAsync(cancellationToken);
        foreach (var line in content.Split('\n'))
            yield return new Student(line, DateTime.Now.ToString(CultureInfo.InvariantCulture) + Guid.NewGuid());
        
        yield return new Student("I was born from file", "File" + Guid.NewGuid());
    }
}