// First run
// | Method                            | Mean     | Error    | StdDev   | Gen0   | Allocated |
// |---------------------------------- |---------:|---------:|---------:|-------:|----------:|
// | RunGeneratorsAndUpdateCompilation | 883.3 us | 16.94 us | 19.51 us | 5.8594 |  58.86 KB |

namespace Benchmarks;

public class ALotOfControllersGeneratorBenchmark
    : ControllerGeneratorBenchmarksBase<ALotOfControllersGeneratorBenchmark.CodeProvider>
{
    public class CodeProvider : ICodeProvider
    {
        public static string Code => """
            using System.Threading;
            using System.Threading.Tasks;
            using AutoApiGen.Attributes;
            using Mediator;
            using static Microsoft.AspNetCore.Http.StatusCodes;
            
            namespace TestConsumer.Features.Books
            {
                public record Book(int Id, string Title, string Author);
            
                [PostEndpoint("books", SuccessCode = Status201Created)]
                public record CreateBookCommand(string Title, string Author) : IRequest<Book>;
            
                public class CreateBookHandler : IRequestHandler<CreateBookCommand, Book>
                {
                    public ValueTask<Book> Handle(CreateBookCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Book(1, command.Title, command.Author));
                }
            
                [GetEndpoint("books/{Id:int}")]
                public record GetBookByIdQuery(int Id) : IRequest<Book>;
            
                public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Book>
                {
                    public ValueTask<Book> Handle(GetBookByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Book(query.Id, "Sample Title", "Sample Author"));
                }
            }
            
            namespace TestConsumer.Features.Authors
            {
                public record Author(int Id, string Name);
            
                [PostEndpoint("authors", SuccessCode = Status201Created)]
                public record CreateAuthorCommand(string Name) : IRequest<Author>;
            
                public class CreateAuthorHandler : IRequestHandler<CreateAuthorCommand, Author>
                {
                    public ValueTask<Author> Handle(CreateAuthorCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Author(1, command.Name));
                }
            
                [GetEndpoint("authors/{Id:int}")]
                public record GetAuthorByIdQuery(int Id) : IRequest<Author>;
            
                public class GetAuthorByIdHandler : IRequestHandler<GetAuthorByIdQuery, Author>
                {
                    public ValueTask<Author> Handle(GetAuthorByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Author(query.Id, "John Doe"));
                }
            }
            
            namespace TestConsumer.Features.Categories
            {
                public record Category(int Id, string Name);
            
                [PostEndpoint("categories", SuccessCode = Status201Created)]
                public record CreateCategoryCommand(string Name) : IRequest<Category>;
            
                public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Category>
                {
                    public ValueTask<Category> Handle(CreateCategoryCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Category(1, command.Name));
                }
            
                [GetEndpoint("categories/{Id:int}")]
                public record GetCategoryByIdQuery(int Id) : IRequest<Category>;
            
                public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Category>
                {
                    public ValueTask<Category> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Category(query.Id, "Fiction"));
                }
            }
            
            namespace TestConsumer.Features.Orders
            {
                public record Order(int Id, string CustomerName, decimal TotalAmount);
            
                [PostEndpoint("orders", SuccessCode = Status201Created)]
                public record CreateOrderCommand(string CustomerName, decimal TotalAmount) : IRequest<Order>;
            
                public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
                {
                    public ValueTask<Order> Handle(CreateOrderCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Order(1, command.CustomerName, command.TotalAmount));
                }
            
                [GetEndpoint("orders/{Id:int}")]
                public record GetOrderByIdQuery(int Id) : IRequest<Order>;
            
                public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order>
                {
                    public ValueTask<Order> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Order(query.Id, "Jane Doe", 99.99m));
                }
            }
            
            namespace TestConsumer.Features.Customers
            {
                public record Customer(int Id, string Name);
            
                [PostEndpoint("customers", SuccessCode = Status201Created)]
                public record CreateCustomerCommand(string Name) : IRequest<Customer>;
            
                public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Customer>
                {
                    public ValueTask<Customer> Handle(CreateCustomerCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Customer(1, command.Name));
                }
            
                [GetEndpoint("customers/{Id:int}")]
                public record GetCustomerByIdQuery(int Id) : IRequest<Customer>;
            
                public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, Customer>
                {
                    public ValueTask<Customer> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Customer(query.Id, "Alice Smith"));
                }
            }
            
            namespace TestConsumer.Features.Products
            {
                public record Product(int Id, string Name, decimal Price);
            
                [PostEndpoint("products", SuccessCode = Status201Created)]
                public record CreateProductCommand(string Name, decimal Price) : IRequest<Product>;
            
                public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
                {
                    public ValueTask<Product> Handle(CreateProductCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Product(1, command.Name, command.Price));
                }
            
                [GetEndpoint("products/{Id:int}")]
                public record GetProductByIdQuery(int Id) : IRequest<Product>;
            
                public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product>
                {
                    public ValueTask<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Product(query.Id, "Product Name", 49.99m));
                }
            }
            
            namespace TestConsumer.Features.Employees
            {
                public record Employee(int Id, string Name, string Position);
            
                [PostEndpoint("employees", SuccessCode = Status201Created)]
                public record CreateEmployeeCommand(string Name, string Position) : IRequest<Employee>;
            
                public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Employee>
                {
                    public ValueTask<Employee> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Employee(1, command.Name, command.Position));
                }
            
                [GetEndpoint("employees/{Id:int}")]
                public record GetEmployeeByIdQuery(int Id) : IRequest<Employee>;
            
                public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, Employee>
                {
                    public ValueTask<Employee> Handle(GetEmployeeByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Employee(query.Id, "John Employee", "Developer"));
                }
            }
            
            namespace TestConsumer.Features.Departments
            {
                public record Department(int Id, string Name);
            
                [PostEndpoint("departments", SuccessCode = Status201Created)]
                public record CreateDepartmentCommand(string Name) : IRequest<Department>;
            
                public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, Department>
                {
                    public ValueTask<Department> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Department(1, command.Name));
                }
            
                [GetEndpoint("departments/{Id:int}")]
                public record GetDepartmentByIdQuery(int Id) : IRequest<Department>;
            
                public class GetDepartmentByIdHandler : IRequestHandler<GetDepartmentByIdQuery, Department>
                {
                    public ValueTask<Department> Handle(GetDepartmentByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Department(query.Id, "HR"));
                }
            }
            
            namespace TestConsumer.Features.Payments
            {
                public record Payment(int Id, decimal Amount, string PaymentMethod);
            
                [PostEndpoint("payments", SuccessCode = Status201Created)]
                public record CreatePaymentCommand(decimal Amount, string PaymentMethod) : IRequest<Payment>;
            
                public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, Payment>
                {
                    public ValueTask<Payment> Handle(CreatePaymentCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Payment(1, command.Amount, command.PaymentMethod));
                }
            
                [GetEndpoint("payments/{Id:int}")]
                public record GetPaymentByIdQuery(int Id) : IRequest<Payment>;
            
                public class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQuery, Payment>
                {
                    public ValueTask<Payment> Handle(GetPaymentByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Payment(query.Id, 150.00m, "Credit Card"));
                }
            }
            
            namespace TestConsumer.Features.Shipments
            {
                public record Shipment(int Id, string Address, DateTime ShippedDate);
            
                [PostEndpoint("shipments", SuccessCode = Status201Created)]
                public record CreateShipmentCommand(string Address, DateTime ShippedDate) : IRequest<Shipment>;
            
                public class CreateShipmentHandler : IRequestHandler<CreateShipmentCommand, Shipment>
                {
                    public ValueTask<Shipment> Handle(CreateShipmentCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Shipment(1, command.Address, command.ShippedDate));
                }
            
                [GetEndpoint("shipments/{Id:int}")]
                public record GetShipmentByIdQuery(int Id) : IRequest<Shipment>;
            
                public class GetShipmentByIdHandler : IRequestHandler<GetShipmentByIdQuery, Shipment>
                {
                    public ValueTask<Shipment> Handle(GetShipmentByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Shipment(query.Id, "123 Main St", DateTime.UtcNow));
                }
            }
            
            namespace TestConsumer.Features.Tickets
            {
                public record Ticket(int Id, string Issue, string Status);
            
                [PostEndpoint("tickets", SuccessCode = Status201Created)]
                public record CreateTicketCommand(string Issue) : IRequest<Ticket>;
            
                public class CreateTicketHandler : IRequestHandler<CreateTicketCommand, Ticket>
                {
                    public ValueTask<Ticket> Handle(CreateTicketCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Ticket(1, command.Issue, "Open"));
                }
            
                [GetEndpoint("tickets/{Id:int}")]
                public record GetTicketByIdQuery(int Id) : IRequest<Ticket>;
            
                public class GetTicketByIdHandler : IRequestHandler<GetTicketByIdQuery, Ticket>
                {
                    public ValueTask<Ticket> Handle(GetTicketByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Ticket(query.Id, "System Crash", "Resolved"));
                }
            }
            
            namespace TestConsumer.Features.Reviews
            {
                public record Review(int Id, int ProductId, string Content);
            
                [PostEndpoint("reviews", SuccessCode = Status201Created)]
                public record CreateReviewCommand(int ProductId, string Content) : IRequest<Review>;
            
                public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, Review>
                {
                    public ValueTask<Review> Handle(CreateReviewCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Review(1, command.ProductId, command.Content));
                }
            
                [GetEndpoint("reviews/{Id:int}")]
                public record GetReviewByIdQuery(int Id) : IRequest<Review>;
            
                public class GetReviewByIdHandler : IRequestHandler<GetReviewByIdQuery, Review>
                {
                    public ValueTask<Review> Handle(GetReviewByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Review(query.Id, 1, "Great product!"));
                }
            }
            
            namespace TestConsumer.Features.Messages
            {
                public record Message(int Id, string Content, string Sender);
            
                [PostEndpoint("messages", SuccessCode = Status201Created)]
                public record CreateMessageCommand(string Content, string Sender) : IRequest<Message>;
            
                public class CreateMessageHandler : IRequestHandler<CreateMessageCommand, Message>
                {
                    public ValueTask<Message> Handle(CreateMessageCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Message(1, command.Content, command.Sender));
                }
            
                [GetEndpoint("messages/{Id:int}")]
                public record GetMessageByIdQuery(int Id) : IRequest<Message>;
            
                public class GetMessageByIdHandler : IRequestHandler<GetMessageByIdQuery, Message>
                {
                    public ValueTask<Message> Handle(GetMessageByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Message(query.Id, "Hello!", "Alice"));
                }
            }
            
            namespace TestConsumer.Features.Notifications
            {
                public record Notification(int Id, string Message, DateTime SentDate);
            
                [PostEndpoint("notifications", SuccessCode = Status201Created)]
                public record CreateNotificationCommand(string Message) : IRequest<Notification>;
            
                public class CreateNotificationHandler : IRequestHandler<CreateNotificationCommand, Notification>
                {
                    public ValueTask<Notification> Handle(CreateNotificationCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Notification(1, command.Message, DateTime.UtcNow));
                }
            
                [GetEndpoint("notifications/{Id:int}")]
                public record GetNotificationByIdQuery(int Id) : IRequest<Notification>;
            
                public class GetNotificationByIdHandler : IRequestHandler<GetNotificationByIdQuery, Notification>
                {
                    public ValueTask<Notification> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Notification(query.Id, "You have a new message.", DateTime.UtcNow));
                }
            }
            
            namespace TestConsumer.Features.Events
            {
                public record Event(int Id, string Title, DateTime Date);
            
                [PostEndpoint("events", SuccessCode = Status201Created)]
                public record CreateEventCommand(string Title, DateTime Date) : IRequest<Event>;
            
                public class CreateEventHandler : IRequestHandler<CreateEventCommand, Event>
                {
                    public ValueTask<Event> Handle(CreateEventCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Event(1, command.Title, command.Date));
                }
            
                [GetEndpoint("events/{Id:int}")]
                public record GetEventByIdQuery(int Id) : IRequest<Event>;
            
                public class GetEventByIdHandler : IRequestHandler<GetEventByIdQuery, Event>
                {
                    public ValueTask<Event> Handle(GetEventByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Event(query.Id, "Conference", DateTime.UtcNow));
                }
            }
            
            namespace TestConsumer.Features.Payrolls
            {
                public record Payroll(int Id, int EmployeeId, decimal Amount);
            
                [PostEndpoint("payrolls", SuccessCode = Status201Created)]
                public record CreatePayrollCommand(int EmployeeId, decimal Amount) : IRequest<Payroll>;
            
                public class CreatePayrollHandler : IRequestHandler<CreatePayrollCommand, Payroll>
                {
                    public ValueTask<Payroll> Handle(CreatePayrollCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Payroll(1, command.EmployeeId, command.Amount));
                }
            
                [GetEndpoint("payrolls/{Id:int}")]
                public record GetPayrollByIdQuery(int Id) : IRequest<Payroll>;
            
                public class GetPayrollByIdHandler : IRequestHandler<GetPayrollByIdQuery, Payroll>
                {
                    public ValueTask<Payroll> Handle(GetPayrollByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Payroll(query.Id, 1, 2000.00m));
                }
            }
            
            namespace TestConsumer.Features.Blogs
            {
                public record Blog(int Id, string Title, string Content);
            
                [PostEndpoint("blogs", SuccessCode = Status201Created)]
                public record CreateBlogCommand(string Title, string Content) : IRequest<Blog>;
            
                public class CreateBlogHandler : IRequestHandler<CreateBlogCommand, Blog>
                {
                    public ValueTask<Blog> Handle(CreateBlogCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Blog(1, command.Title, command.Content));
                }
            
                [GetEndpoint("blogs/{Id:int}")]
                public record GetBlogByIdQuery(int Id) : IRequest<Blog>;
            
                public class GetBlogByIdHandler : IRequestHandler<GetBlogByIdQuery, Blog>
                {
                    public ValueTask<Blog> Handle(GetBlogByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Blog(query.Id, "Blog Title", "Blog Content"));
                }
            }
            
            namespace TestConsumer.Features.Admins
            {
                public record Admin(int Id, string Username);
            
                [PostEndpoint("admins", SuccessCode = Status201Created)]
                public record CreateAdminCommand(string Username) : IRequest<Admin>;
            
                public class CreateAdminHandler : IRequestHandler<CreateAdminCommand, Admin>
                {
                    public ValueTask<Admin> Handle(CreateAdminCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Admin(1, command.Username));
                }
            
                [GetEndpoint("admins/{Id:int}")]
                public record GetAdminByIdQuery(int Id) : IRequest<Admin>;
            
                public class GetAdminByIdHandler : IRequestHandler<GetAdminByIdQuery, Admin>
                {
                    public ValueTask<Admin> Handle(GetAdminByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Admin(query.Id, "admin_user"));
                }
            }
            
            namespace TestConsumer.Features.Files
            {
                public record File(int Id, string FileName, long Size);
            
                [PostEndpoint("files", SuccessCode = Status201Created)]
                public record UploadFileCommand(string FileName, long Size) : IRequest<File>;
            
                public class UploadFileHandler : IRequestHandler<UploadFileCommand, File>
                {
                    public ValueTask<File> Handle(UploadFileCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new File(1, command.FileName, command.Size));
                }
            
                [GetEndpoint("files/{Id:int}")]
                public record GetFileByIdQuery(int Id) : IRequest<File>;
            
                public class GetFileByIdHandler : IRequestHandler<GetFileByIdQuery, File>
                {
                    public ValueTask<File> Handle(GetFileByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new File(query.Id, "document.pdf", 1024));
                }
            }
            
            namespace TestConsumer.Features.Comments
            {
                public record Comment(int Id, int PostId, string Content);
            
                [PostEndpoint("comments", SuccessCode = Status201Created)]
                public record CreateCommentCommand(int PostId, string Content) : IRequest<Comment>;
            
                public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Comment>
                {
                    public ValueTask<Comment> Handle(CreateCommentCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Comment(1, command.PostId, command.Content));
                }
            
                [GetEndpoint("comments/{Id:int}")]
                public record GetCommentByIdQuery(int Id) : IRequest<Comment>;
            
                public class GetCommentByIdHandler : IRequestHandler<GetCommentByIdQuery, Comment>
                {
                    public ValueTask<Comment> Handle(GetCommentByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Comment(query.Id, 1, "This is a comment."));
                }
            }
            
            namespace TestConsumer.Features.Permissions
            {
                public record Permission(int Id, string Role, string Action);
            
                [PostEndpoint("permissions", SuccessCode = Status201Created)]
                public record CreatePermissionCommand(string Role, string Action) : IRequest<Permission>;
            
                public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, Permission>
                {
                    public ValueTask<Permission> Handle(CreatePermissionCommand command, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Permission(1, command.Role, command.Action));
                }
            
                [GetEndpoint("permissions/{Id:int}")]
                public record GetPermissionByIdQuery(int Id) : IRequest<Permission>;
            
                public class GetPermissionByIdHandler : IRequestHandler<GetPermissionByIdQuery, Permission>
                {
                    public ValueTask<Permission> Handle(GetPermissionByIdQuery query, CancellationToken cancellationToken) =>
                        ValueTask.FromResult(new Permission(query.Id, "Admin", "Can manage users and roles."));
                }
            }
            """;
    }
}
