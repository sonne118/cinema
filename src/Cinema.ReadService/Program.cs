using Cinema.Infrastructure;
using Cinema.Infrastructure.Messaging;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();


builder.Services.AddInfrastructure(builder.Configuration, isWriteSide: false);


builder.Services.AddHostedService<KafkaConsumer>();

var host = builder.Build();
host.Run();
