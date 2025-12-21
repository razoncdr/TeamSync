using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;
using TeamSync.API.Hubs;
using TeamSync.API.Middleware;
using TeamSync.API.Realtime;
using TeamSync.Application.Events;
using TeamSync.Application.Interfaces.Repositories;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Application.Services;
using TeamSync.Infrastructure;
using TeamSync.Infrastructure.Messaging;
using TeamSync.Infrastructure.Messaging.Consumers;
using TeamSync.Infrastructure.Repositories;
using TeamSync.Infrastructure.Services;
using TeamSync.Infrastructure.Settings;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});


// Configure settings
builder.Services.Configure<JWTSettings>(
	builder.Configuration.GetSection("JWTSettings")
);

builder.Services.Configure<MongoDBSettings>(
	builder.Configuration.GetSection("MongoDBSettings")
);

builder.Services.Configure<RabbitMqSettings>(
	builder.Configuration.GetSection("RabbitMq")
);

builder.Services.AddSingleton(provider =>
{
	var settings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>();
	return new RabbitMqEventPublisher(settings);
});

builder.Services.AddSingleton<IEventPublisher>(provider =>
	provider.GetRequiredService<RabbitMqEventPublisher>()
);

builder.Services.AddSingleton<RabbitMqSettings>();


// RabbitMQ background consumers
builder.Services.AddHostedService<ProjectCreatedConsumer>();
builder.Services.AddHostedService<ProjectUpdatedConsumer>();

// Infrastructure layer DI
builder.Services.AddInfrastructure(builder.Configuration);

// Dependency Injection for services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectMemberService, ProjectMemberService>();
builder.Services.AddScoped<IProjectInvitationService, ProjectInvitationService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskItemRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<IProjectInvitationRepository, ProjectInvitationRepository>();

builder.Services.AddScoped<IChatNotifier, SignalRChatNotifier>();

var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
	ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddSignalR();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSettings.Issuer,
			ValidAudience = jwtSettings.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
		};


    });

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeamSync API", Version = "v1" });

	// Configure JWT Authorization in Swagger
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter 'Bearer' [space] and then your JWT token"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] { }
		}
	});
});

var app = builder.Build();
//app.UseCors();
app.UseCors("CorsPolicy");


// Swagger in Development
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionMiddleware();

// 🔑 Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
