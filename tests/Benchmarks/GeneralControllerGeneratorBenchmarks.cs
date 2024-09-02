namespace Benchmarks;

public class GeneralControllerGeneratorBenchmarks 
    : ControllerGeneratorBenchmarksBase<GeneralControllerGeneratorBenchmarks.CodeProvider>
{
    public class CodeProvider : ICodeProvider
    {
        public static string Code => """"
            using System.Threading;
            using System.Threading.Tasks;
            using AutoApiGen.Attributes;
            using Mediator;
            using static Microsoft.AspNetCore.Http.StatusCodes;

            namespace TestConsumer.Features.Contests;

            public record Contest(int Id, string Name, bool Status, List<Participant> Participants);

            public record Participant(int Id, string Name);

            public record Course(int Id, string Title, string Description, List<Instructor> Instructors);
            
            public record Instructor(int Id, string Name);
            
            public record Student(string Name, string Id);
            
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

            public static class UpdateContest
            {
                [PutEndpoint("contests/{Id:int}",
                    SuccessCode = StatusCodes.Status204NoContent)]
                public record Command(int Id, string Name, List<Participant> Participants) : ICommand;
                
                public class Handler : ICommandHandler<Command>
                {
                    public ValueTask<Unit> Handle(Command request, CancellationToken cancellationToken) => 
                        Unit.ValueTask;
                }
            }

            public static class RootQuery
            {
                [GetEndpoint]
                public record Query : IQuery<string>;
            
                public class RootQueryHandler : IQueryHandler<Query, string>
                {
                    public ValueTask<string> Handle(Query query, CancellationToken cancellationToken) => 
                        ValueTask.FromResult("Welcome to AutoApiGen TempConsumer project!");
                }
            }

            public static class GetContestByName
            {
                [GetEndpoint("contests/{Name}",
                    ErrorCode = StatusCodes.Status400BadRequest
                )]
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

            public static class GetContest
            {
                [GetEndpoint("contests/{Id:int}")]
                public record Query(int Id) : IRequest<Contest>;
                
                public class Handler : IRequestHandler<Query, Contest>
                {
                    public ValueTask<Contest> Handle(Query query, CancellationToken cancellationToken)
                    {
                        var id = query.Id;
            
                        return ValueTask.FromResult(new Contest(id, "Contest", true, [new Participant(25, "Daniel")]));
                    }
                }
            }
            
            [PostEndpoint("students",
                SuccessCode = StatusCodes.Status201Created,
                ErrorCode = StatusCodes.Status403Forbidden
            )] 
            public record CreateStudentCommand(string Name) : ICommand<OneOf<Student, Error>>;

            public class CreateStudentHandler(StudentsRepo repo) 
                : ICommandHandler<CreateStudentCommand, OneOf<Student, Error>>
            {
                public async ValueTask<OneOf<Student, Error>> Handle(
                    CreateStudentCommand command,
                    CancellationToken cancellationToken
                )
                {
                    await ValueTask.CompletedTask;
            
                    var name = command.Name;
            
                    if (name.Equals("Badam", StringComparison.OrdinalIgnoreCase))
                        return Error.Forbidden();
                    
                    var student = new Student(name, Random.Shared.Next().ToString());
            
                    repo.Add(student);
            
                    return student;
                }
            }

            [GetEndpoint("students/{Id}")] 
            public record GetStudentQuery(string Id) : IQuery<Student?>;

            public class GetStudentHandler(StudentsRepo repo) : IQueryHandler<GetStudentQuery, Student?>
            {
                public ValueTask<Student?> Handle(GetStudentQuery query, CancellationToken cancellationToken) => 
                    ValueTask.FromResult(repo.Get(query.Id));
            }

            public class StudentsRepo
            {
                private readonly Dictionary<string, Student> _students = [];
            
                public void Add(Student student) => 
                    _students.Add(student.Id, student);
            
                public Student? Get(string name) =>
                    _students.GetValueOrDefault(name);
            
                public List<Student> GetAll() =>
                    _students.Values.ToList();
            }
            
            public static class CreateCourse
            {
                [PostEndpoint("courses/{Id:int=100}",
                    SuccessCode = Status201Created)]
                public record Command(
                    string Title,
                    string Description,
                    int Id = 100
                ) : IRequest<Course>;
        
                public class Handler : IRequestHandler<Command, Course>
                {
                    public ValueTask<Course> Handle(Command request, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Course(request.Id, request.Title, request.Description, []));
                }
            }
        
            public static class UpdateCourse
            {
                [PutEndpoint("courses/{Id:int}",
                    SuccessCode = Status204NoContent)]
                public record Command(int Id, string Title, string Description) : ICommand;
        
                public class Handler : ICommandHandler<Command>
                {
                    public ValueTask<Unit> Handle(Command request, CancellationToken cancellationToken) => 
                        Unit.ValueTask;
                }
            }
        
            public static class GetCourseById
            {
                [GetEndpoint("courses/{Id:int}")]
                public record Query(int Id) : IQuery<Course>;
        
                public class Handler : IQueryHandler<Query, Course>
                {
                    public ValueTask<Course> Handle(Query query, CancellationToken cancellationToken)
                    {
                        var id = query.Id;
                        return ValueTask.FromResult(new Course(id, "Sample Course", "A sample description", []));
                    }
                }
            }
        
            public static class GetAllCourses
            {
                [GetEndpoint("courses")]
                public record Query : IQuery<List<Course>>;
        
                public class Handler : IQueryHandler<Query, List<Course>>
                {
                    public ValueTask<List<Course>> Handle(Query query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new List<Course>
                        {
                            new Course(1, "Course 1", "Description 1", []),
                            new Course(2, "Course 2", "Description 2", [])
                        });
                }
            }
        
            public static class AddInstructorToCourse
            {
                [PostEndpoint("courses/{CourseId:int}/instructors",
                    SuccessCode = Status201Created)]
                public record Command(int CourseId, string InstructorName) : IRequest<Instructor>;
        
                public class Handler : IRequestHandler<Command, Instructor>
                {
                    public ValueTask<Instructor> Handle(Command request, CancellationToken cancellationToken)
                    {
                        var instructor = new Instructor(10, request.InstructorName);
                        // Assume that the instructor is added to the course here.
                        return ValueTask.FromResult(instructor);
                    }
                }
            }
        
            public static class CreateInstructor
            {
                [PostEndpoint("instructors",
                    SuccessCode = Status201Created)]
                public record Command(string Name) : IRequest<Instructor>;
        
                public class Handler : IRequestHandler<Command, Instructor>
                {
                    public ValueTask<Instructor> Handle(Command request, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Instructor(1, request.Name));
                }
            }
        
            public static class GetInstructorById
            {
                [GetEndpoint("instructors/{Id:int}")]
                public record Query(int Id) : IQuery<Instructor>;
        
                public class Handler : IQueryHandler<Query, Instructor>
                {
                    public ValueTask<Instructor> Handle(Query query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Instructor(query.Id, "John Doe"));
                }
            }
        
            public static class GetAllInstructors
            {
                [GetEndpoint("instructors")]
                public record Query : IQuery<List<Instructor>>;
        
                public class Handler : IQueryHandler<Query, List<Instructor>>
                {
                    public ValueTask<List<Instructor>> Handle(Query query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new List<Instructor>
                        {
                            new Instructor(1, "Instructor 1"),
                            new Instructor(2, "Instructor 2")
                        });
                }
            }
            
            [assembly: AutoApiGen.ConfigAttributes.SetMediatorPackage("global::Mediator")]
            [assembly: AutoApiGen.ConfigAttributes.ResultTypeConfiguration(
                TypeName = "OneOf",
                MatchMethodName = "Match",
                ErrorHandlerMethodName = "Problem",
                ErrorHandlerMethodImplementation = """
                private global::Microsoft.AspNetCore.Mvc.IActionResult Problem(
                    global::System.Collections.Generic.List<global::ErrorOr.Error> errors
                ) => Problem(
                    statusCode: errors[0].Type switch
                    {
                        global::ErrorOr.ErrorType.Conflict => 409,
                        global::ErrorOr.ErrorType.Validation => 400,
                        global::ErrorOr.ErrorType.NotFound => 404,
                        global::ErrorOr.ErrorType.Forbidden => 403,
                        _ => 500
                    },
                    title: errors[0].Description
                );
                
                private global::Microsoft.AspNetCore.Mvc.IActionResult Problem(
                    global::ErrorOr.Error error
                ) => Problem(
                    statusCode: error.Type switch
                    {
                        global::ErrorOr.ErrorType.Conflict => 409,
                        global::ErrorOr.ErrorType.Validation => 400,
                        global::ErrorOr.ErrorType.NotFound => 404,
                        global::ErrorOr.ErrorType.Forbidden => 403,
                        _ => 500
                    },
                    title: error.Description
                );
                """
            )]
            """";
    }
}