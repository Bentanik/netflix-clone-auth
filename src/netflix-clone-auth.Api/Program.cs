var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        option =>
        {
            option.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


builder.AddApplicationServices();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.ConfigureMiddleware();

app.Run();
