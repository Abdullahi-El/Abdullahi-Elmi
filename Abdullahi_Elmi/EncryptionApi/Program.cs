using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Encryption API",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Encryption API v1");
    });
}

app.UseHttpsRedirection();

// Caesar cipher: Kryptering och Avkryptering
int shift = 3; // Steg för kryptering

string Encrypt(string text, int shift)
{
    return new string(text.Select(c => char.IsLetter(c)
        ? (char)(((c - (char.IsUpper(c) ? 'A' : 'a') + shift + 26) % 26) + (char.IsUpper(c) ? 'A' : 'a'))
        : c).ToArray());
}

string Decrypt(string text, int shift)
{
    return Encrypt(text, 26 - shift); // Dekryptering är kryptering med ett negativt steg
}

// Endpoint för kryptering
app.MapPost("/encrypt", (string plaintext) =>
{
    var encrypted = Encrypt(plaintext, shift);
    return Results.Ok(new { plaintext, encrypted });
})
.WithName("EncryptText");

// Endpoint för avkryptering
app.MapPost("/decrypt", (string ciphertext) =>
{
    var decrypted = Decrypt(ciphertext, shift);
    return Results.Ok(new { ciphertext, decrypted });
})
.WithName("DecryptText");

// Weather forecast endpoint (behåller din ursprungliga funktionalitet)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
