using TaskFlow.Application;
using TaskFlow.Infrastructure;
using TaskFlow.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddPresistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Swagger pipeline (ONLY in development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();