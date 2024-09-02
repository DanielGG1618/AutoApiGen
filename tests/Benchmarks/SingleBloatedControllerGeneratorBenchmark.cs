namespace Benchmarks;

public class SingleBloatedControllerGeneratorBenchmark
    : ControllerGeneratorBenchmarksBase<SingleBloatedControllerGeneratorBenchmark.CodeProvider>
{
    public class CodeProvider : ICodeProvider
    {
        public static string Code => """"
            using System.Threading;
            using System.Threading.Tasks;
            using AutoApiGen.Attributes;
            using Mediator;
            using static Microsoft.AspNetCore.Http.StatusCodes;

            namespace TestConsumer.Features.Users
            {
                public record User(int Id, string Username, string Email, string Role);
            
                [PostEndpoint("users", SuccessCode = Status201Created)]
                public record CreateUserCommand(string Username, string Email, string Role) : IRequest<User>;
            
                public class CreateUserHandler : IRequestHandler<CreateUserCommand, User>
                {
                    public ValueTask<User> Handle(CreateUserCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(1, command.Username, command.Email, command.Role));
                }
            
                [GetEndpoint("users/{Id:int}")]
                public record GetUserByIdQuery(int Id) : IRequest<User>;
            
                public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, User>
                {
                    public ValueTask<User> Handle(GetUserByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(query.Id, "SampleUser", "sample@example.com", "User"));
                }
            
                [GetEndpoint("users")]
                public record GetAllUsersQuery : IRequest<User[]>;
            
                public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, User[]>
                {
                    public ValueTask<User[]> Handle(GetAllUsersQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new[]
                        {
                            new User(1, "User1", "user1@example.com", "Admin"),
                            new User(2, "User2", "user2@example.com", "User")
                        });
                }
            
                [PutEndpoint("users/{Id:int}")]
                public record UpdateUserCommand(int Id, string Username, string Email, string Role) : IRequest<User>;
            
                public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User>
                {
                    public ValueTask<User> Handle(UpdateUserCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(command.Id, command.Username, command.Email, command.Role));
                }
            
                [DeleteEndpoint("users/{Id:int}")]
                public record DeleteUserCommand(int Id) : IRequest;
            
                public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
                {
                    public ValueTask<Unit> Handle(DeleteUserCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(Unit.Value);
                }
            
                [GetEndpoint("users/{Id:int}/roles")]
                public record GetUserRolesQuery(int Id) : IRequest<string[]>;
            
                public class GetUserRolesHandler : IRequestHandler<GetUserRolesQuery, string[]>
                {
                    public ValueTask<string[]> Handle(GetUserRolesQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new[] { "Admin", "User" });
                }
            
                [PostEndpoint("users/{Id:int}/roles")]
                public record AddUserRoleCommand(int Id, string Role) : IRequest<User>;
            
                public class AddUserRoleHandler : IRequestHandler<AddUserRoleCommand, User>
                {
                    public ValueTask<User> Handle(AddUserRoleCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(command.Id, "User1", "user1@example.com", command.Role));
                }
            
                [DeleteEndpoint("users/{Id:int}/roles")]
                public record RemoveUserRoleCommand(int Id, string Role) : IRequest<User>;
            
                public class RemoveUserRoleHandler : IRequestHandler<RemoveUserRoleCommand, User>
                {
                    public ValueTask<User> Handle(RemoveUserRoleCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(command.Id, "User1", "user1@example.com", "User"));
                }
            
                [PatchEndpoint("users/{Id:int}/password")]
                public record ChangeUserPasswordCommand(int Id, string NewPassword) : IRequest;
            
                public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand>
                {
                    public ValueTask<Unit> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(Unit.Value);
                }
            
                [PostEndpoint("users/{Id:int}/activate")]
                public record ActivateUserCommand(int Id) : IRequest<User>;
            
                public class ActivateUserHandler : IRequestHandler<ActivateUserCommand, User>
                {
                    public ValueTask<User> Handle(ActivateUserCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(command.Id, "User1", "user1@example.com", "Active"));
                }
            
                [PostEndpoint("users/{Id:int}/deactivate")]
                public record DeactivateUserCommand(int Id) : IRequest<User>;
            
                public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, User>
                {
                    public ValueTask<User> Handle(DeactivateUserCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(command.Id, "User1", "user1@example.com", "Inactive"));
                }
            
                [GetEndpoint("users/{Id:int}/profile")]
                public record GetUserProfileQuery(int Id) : IRequest<User>;
            
                public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, User>
                {
                    public ValueTask<User> Handle(GetUserProfileQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(query.Id, "User1", "user1@example.com", "User"));
                }
            
                [PutEndpoint("users/{Id:int}/profile")]
                public record UpdateUserProfileCommand(int Id, string Email, string Username) : IRequest<User>;
            
                public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, User>
                {
                    public ValueTask<User> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new User(command.Id, command.Username, command.Email, "User"));
                }
            
                [PostEndpoint("users/{Id:int}/reset-password")]
                public record ResetUserPasswordCommand(int Id) : IRequest;
            
                public class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordCommand>
                {
                    public ValueTask<Unit> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(Unit.Value);
                }
            
                [GetEndpoint("users/{Id:int}/activity")]
                public record GetUserActivityQuery(int Id) : IRequest<string[]>;
            
                public class GetUserActivityHandler : IRequestHandler<GetUserActivityQuery, string[]>
                {
                    public ValueTask<string[]> Handle(GetUserActivityQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new[] { "Logged in", "Updated profile" });
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


