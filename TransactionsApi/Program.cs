var builder = WebApplication.CreateBuilder(args);

var apiVersion = builder.Configuration["Api:Version"] ?? "v1";

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks($"/api/{apiVersion}/health");

app.Run();
