using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Endpoints
{
    public static class WeatherForecastEndpoints
    {
        internal static void MapWeatherForecastEndpoints(this IEndpointRouteBuilder app)
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", () =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateTime.Now.AddDays(index),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();

                return forecast;
            })
            .WithName("GetWeatherForecast");
        }
    }
}
