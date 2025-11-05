using DotNetEnv;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Application.Services;
using TeamSync.Infrastructure;
using TeamSync.Infrastructure.Settings;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
//Console.WriteLine(builder.Configuration["MongoDBSettings:ConnectionString"]);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDBSettings>(
	builder.Configuration.GetSection("MongoDBSettings")
);
// Infrastructure layer DI
builder.Services.AddInfrastructure(builder.Configuration);

// Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
