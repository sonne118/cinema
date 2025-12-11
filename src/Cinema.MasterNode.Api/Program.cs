using Cinema.MasterNode.Core.Services;
using Cinema.MasterNode.Infrastructure.Data;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddDbContextPool<MasterDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MasterDb"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            sqlOptions.CommandTimeout(60);
            sqlOptions.MaxBatchSize(100);
        }), poolSize: 128);

builder.Services.AddSingleton(sp => 
{
    var config = builder.Configuration;
    var producerConfig = new ProducerConfig
    {
        BootstrapServers = config["Kafka:BootstrapServers"],
        Acks = Acks.All, 
        EnableIdempotence = true, 
        MaxInFlight = 5,
        CompressionType = CompressionType.Snappy,
        LingerMs = 10, 
        BatchSize = 1048576, 
        MessageSendMaxRetries = 3,
        RetryBackoffMs = 100
    };
    
    return new ProducerBuilder<string, string>(producerConfig)
        .SetErrorHandler((_, e) => 
            Console.WriteLine($"Kafka Error: {e.Reason}"))
        .Build();
});


builder.Services.AddSingleton<IOutboxProcessor, OutboxProcessor>();
builder.Services.AddHostedService<MasterNodeWorker>();

var app = builder.Build();

app.MapGet("/", () => "Cinema Master Node Running (Outbox Pattern)");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
