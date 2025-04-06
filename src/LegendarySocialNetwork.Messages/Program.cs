using Asp.Versioning;
using LegendarySocialNetwork.Messages;
using LegendarySocialNetwork.Messages.Database;
using LegendarySocialNetwork.Messages.Filters;
using LegendarySocialNetwork.Messages.Middlewares;
using LegendarySocialNetwork.Messages.Services;
using LegendarySocialNetwork.Messages.Tarantool;
using LegendarySocialNetwork.Messages.Tarantool.HostedService;
using LegendarySocialNetwork.Messages.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using Prometheus;
using System.Text;

var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ValidateModelStateAttribute));
});
builder.Services.AddCors(options => options
.AddDefaultPolicy(corsBuilder => corsBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));
builder.Services.AddHttpContextAccessor();

builder.Services.AddOpenTelemetry()
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["JWTSettings:Issuer"],
            ValidAudience = config["JWTSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTSettings:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };

    });

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.Configure<DatabaseSettings>(config.GetSection("DatabaseSettings"));
config.GetSection(nameof(DatabaseSettings)).Bind(HashUtility.DbSettings);
config.GetSection(nameof(DatabaseSettings)).Bind(DatabaseInitializer.DbSetting);
builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
builder.Services.AddTransient<IDialogService, DialogService>();
builder.Services.AddSingleton<ITarantoolService, TarantoolService>();
builder.Services.AddHostedService<TarantoolReplicatorService>();
builder.Services.AddInfrastracture();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Legendary.Social.Network",
        Description = ""
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
});

var app = builder.Build();
await DatabaseInitializer.Init();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpMetrics();
app.UseMetricServer();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors();
app.MapControllers();
app.MapPrometheusScrapingEndpoint();

app.Run();
