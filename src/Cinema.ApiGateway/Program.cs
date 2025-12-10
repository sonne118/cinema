using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Cinema.Api.Services;
using Cinema.GrpcService;
using System.Net.Http;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<CinemaWriteService.CinemaWriteServiceClient>(o =>
{
    o.Address = new Uri("http://cinema-loadbalancer:80");
})
.ConfigureHttpClient(client =>
{
    client.DefaultRequestVersion = System.Net.HttpVersion.Version20;
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
});

builder.Services.AddGrpcClient<CinemaService.CinemaServiceClient>(o =>
{
    o.Address = new Uri("http://cinema-grpc:5001");
})
.ConfigureHttpClient(client =>
{
    client.DefaultRequestVersion = System.Net.HttpVersion.Version20;
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
