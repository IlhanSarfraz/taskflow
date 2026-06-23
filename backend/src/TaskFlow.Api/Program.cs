using Microsoft.EntityFrameworkCore;
using TaskFlow.Application;
using TaskFlow.Infrastructure;
using TaskFlow.Persistence;
using TaskFlow.Persistence.Context;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();

// Register CORS
builder.Services.AddApplication();
builder.Services.AddPresistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Enable WAL mode for SQLite to reduce write/read lock contention
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
}

// Swagger pipeline (ONLY in development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Global exception middleware
app.UseMiddleware<TaskFlow.Api.Middleware.ExceptionHandlingMiddleware>();

// JWT authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();