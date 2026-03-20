using Microsoft.OpenApi;
using TaskManager.API.Middleware;
using TaskManager.Application.Extensions;
using TaskManager.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManager API",
        Version = "v1",
        Description = "A clean architecture task management API"
    });

    // Tell Swagger about XML doc comments (the /// summaries on controllers)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Pipeline ──────────────────────────────────────────────
var app = builder.Build();

// Exception middleware must be FIRST — wraps everything below it
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
    options.RoutePrefix = string.Empty;  // Swagger opens at root URL
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();