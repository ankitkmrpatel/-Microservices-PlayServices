using Play.Common.Data;
using Play.Common.MassTransit;
using Play.Catalog.Service.Data.Entities;
using Play.Catalog.Service.Data.Repo;
using Play.Catalog.Service.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add Database amd Services
builder.Services.AddMongo(builder.Configuration)
    .AddMongoRepo<Item, ItemRepository>();

// Add Message Brocker
builder.Services.AddMassTransitWithRabbitMQ(builder.Configuration);

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
app.MapCatalogItemEndpoints();
app.MapWeatherForecastEndpoints();

app.Run();