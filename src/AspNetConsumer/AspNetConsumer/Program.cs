using AspNetConsumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddControllers();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<IApiMarker>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

//var a = new ContestsController();
