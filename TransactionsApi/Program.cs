using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using TransactionsApi.Services;
using TransactionsApi.Swagger;

var builder = WebApplication.CreateBuilder(args);

var apiVersionString = builder.Configuration["Api:Version"] ?? "1.0";
var major = Convert.ToInt32(apiVersionString.Split('.')[0]);
var minor = Convert.ToInt32(apiVersionString.Split('.')[1]);
var apiVersion = new ApiVersion(major, minor);


builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();

builder.Services.AddHealthChecks();

builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = apiVersion;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks($"/api/health");

app.MapControllers();

app.Run();
