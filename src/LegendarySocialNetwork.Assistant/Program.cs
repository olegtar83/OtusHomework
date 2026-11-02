using System.ClientModel;
using Dapper;
using LegendarySocialNetwork.Assistant.HostedService;
using LegendarySocialNetwork.Assistant.Service;
using Microsoft.Extensions.AI;
using Microsoft.OpenApi;
using Npgsql;
using OpenAI;
using Pgvector.Dapper;
using Pgvector.Npgsql;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Legendary.Social.Network",
        Description = ""
    });
});

var embeddingsUrl = builder.Configuration["Embeddings:Url"] ?? "http://embeddings:8000/v1";
var embeddingsModel = builder.Configuration["Embeddings:Model"] ?? "intfloat/multilingual-e5-small";

builder.Services.AddEmbeddingGenerator<string, Embedding<float>>(_ =>
    new OpenAIClient(
            new ApiKeyCredential("not-used"),
            new OpenAIClientOptions { Endpoint = new Uri(embeddingsUrl) })
        .GetEmbeddingClient(embeddingsModel)
        .AsIEmbeddingGenerator());

builder.Services.AddHostedService<PgVectorReplicatorService>();
builder.Services.AddScoped<IPgVectorService, PgVectorService>();
builder.Services.AddNpgsqlDataSource(builder.Configuration["DatabaseSettings:MessagesConnStr"], serviceKey: "messages-db");

builder.Services.AddNpgsqlDataSource(builder.Configuration["DatabaseSettings:VectorConnStr"], dataSourceBuilder =>
{
    dataSourceBuilder.AddTypeInfoResolverFactory(new VectorTypeInfoResolverFactory());
}, serviceKey: "pg-vector");

SqlMapper.AddTypeHandler(new VectorTypeHandler());

var app = builder.Build();
await DatabaseInitializer.Init(app.Services.GetKeyedService<NpgsqlDataSource>("pg-vector"));
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
