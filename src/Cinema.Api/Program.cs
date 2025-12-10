using Cinema.Api.Services;
using Cinema.Application;
using Cinema.Infrastructure;
using Cinema.Infrastructure.Persistence.Write;
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core; 
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http2);
});


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


builder.Services.AddControllers();
builder.Services.AddGrpc(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Cinema DDD CQRS API",
        Version = "v1",
        Description = "Clean Architecture with DDD, CQRS, Event Sourcing, Kafka, SQL Server & MongoDB"
    });
});


builder.Services.AddApplication();


builder.Services.AddValidatorsFromAssembly(
    typeof(Program).Assembly);


builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();









if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<CinemaWriteGrpcService>();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
    if (dbContext.Database.IsSqlServer())
    {
        dbContext.Database.Migrate();
    }
}

app.Run();


public partial class Program { }

namespace Cinema.Application
{
    
    public class AssemblyReference { }
}
