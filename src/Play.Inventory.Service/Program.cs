using Play.Common.Data;
using Play.Inventory.Service.Endpoints;
using Play.Inventory.Service.Clients;
using Polly;
using Polly.Timeout;
using Play.Common.MassTransit;
using Play.Inventory.Service.Data.Entities;
using Play.Inventory.Service.Data.Repo;

var builder = WebApplication.CreateBuilder(args);

// Add Database amd Services
builder.Services.AddMongo(builder.Configuration)
    .AddMongoRepo<InventoryItem, InventoryItemRepository>()
    .AddMongoRepo<CatalogItem, CatalogItemRepository>();

builder.Services.AddMassTransitWithRabbitMQ(builder.Configuration);

AddCatalogClient(builder);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(config =>
    {
        config.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapInventoryItemEndpoints();
app.MapWeatherForecastEndpoints();

app.Run();





static void AddCatalogClient(WebApplicationBuilder builder)
{
    Random jitter = new();

    builder.Services.AddHttpClient<ICatalogClinet, CatalogClinet>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:44304");
    })
    .AddTransientHttpErrorPolicy(policyRetrtBuilder =>
        policyRetrtBuilder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5
            , retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitter.Next(1, 1000))
    //,onRetry: (outcome, timespan, retryAttampt) =>
    //{
    //    var serviceProvider = builder.Services.BuildServiceProvider();
    //    Console.WriteLine("Retring Request : at Catalog");
    //    serviceProvider.GetService<ILogger<CatalogClinet>>()?.LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retyr {retryAttampt}");
    //}
    ))
    .AddTransientHttpErrorPolicy(policyCircuitBuilder =>
        policyCircuitBuilder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            3
            , TimeSpan.FromSeconds(10)
    //,onBreak: (outcome, timespan) =>
    //{
    //    var serviceProvider = builder.Services.BuildServiceProvider();
    //    Console.WriteLine("Retring Request : at Catalog");
    //    serviceProvider.GetService<ILogger<CatalogClinet>>()?.LogWarning($"Opening the Circuit for Catalog Service for {timespan.TotalSeconds} seconds");
    //}
    //,onReset: () =>
    //{
    //    var serviceProvider = builder.Services.BuildServiceProvider();
    //    Console.WriteLine("Retring Request : at Catalog");
    //    serviceProvider.GetService<ILogger<CatalogClinet>>()?.LogWarning($"Closint the Circuit for Catalog Service");
    //}
    ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(2));
}