using CachingFramework.Redis;
using CachingFramework.Redis.MsgPack;
using Confluent.Kafka;
using LegendarySocialNetwork.Application.Common.Handlers;
using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Consumers;
using LegendarySocialNetwork.Domain.Messages;
using LegendarySocialNetwork.Infrastructure.Caching;
using LegendarySocialNetwork.Infrastructure.Repositories;
using LegendarySocialNetwork.Infrastructure.Saga;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.MsgPack;

namespace LegendarySocialNetwork.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes => classes.AssignableTo(typeof(IConfigureOptions<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

            services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes => classes.AssignableTo(typeof(IConsumer<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IPostRepository, PostRepository>();

            services.AddScoped<IFeedRepository, FeedRepository>();
            services.Decorate<IFeedRepository, CachedFeedRepository>();

            services.AddScoped(typeof(IListCache<>), typeof(RedisListCache<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));

                x.AddRider(rider =>
                {
                    rider.AddProducer<string, InitSaga>(Environment.GetEnvironmentVariable("Kafka:InitSagaTopic"));
                    rider.AddProducer<string, CreateMessage>(Environment.GetEnvironmentVariable("Kafka:CreateMessageTopic"));
                    rider.AddProducer<string, IncrementCounter>(Environment.GetEnvironmentVariable("Kafka:IncrementCounterTopic"));
                    rider.AddProducer<string, DecrementCounter>(Environment.GetEnvironmentVariable("Kafka:DecrementCounterTopic"));

                    rider.AddConsumer<UpdateCacheFeedConsumer>();
                    rider.AddConsumer<PushFeedWebSocketConsumer>();

                    rider.AddSagaStateMachine<CounterSagaStateMachine, CounterSagaStateInstance>()
                    .RedisRepository(Environment.GetEnvironmentVariable("Redis:ConnectionString"), configure =>
                     {
                         configure.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                         configure.KeyPrefix = "Saga";
                     });

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host(Environment.GetEnvironmentVariable("Kafka:Host"));

                        k.TopicEndpoint<UpdateFeedMessage>(Environment.GetEnvironmentVariable("Kafka:UpdateFeedTopic"),
                            Environment.GetEnvironmentVariable("Kafka:UpdateFeedGroup"), e =>
                        {
                            e.CreateIfMissing(x => x.ReplicationFactor = 1);
                            e.ConfigureConsumer<UpdateCacheFeedConsumer>(context);
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;
                        });

                        k.TopicEndpoint<UpdateFeedMessage>(Environment.GetEnvironmentVariable("Kafka:PushFeedTopic"),
                           Environment.GetEnvironmentVariable("Kafka:PushFeedGroup"), e =>
                           {
                               e.CreateIfMissing(x => x.ReplicationFactor = 1);
                               e.ConfigureConsumer<PushFeedWebSocketConsumer>(context);
                               e.AutoOffsetReset = AutoOffsetReset.Earliest;
                           });

                        k.TopicEndpoint<string, InitSaga>(Environment.GetEnvironmentVariable("Kafka:InitSagaTopic"), $"{Environment.GetEnvironmentVariable("Kafka:InitSagaTopic")}_group", e =>
                        {
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            e.CreateIfMissing(x => x.ReplicationFactor = 1);
                            e.ConfigureSaga<CounterSagaStateInstance>(context);
                            e.DiscardSkippedMessages();
                        });

                        k.TopicEndpoint<string, MessageFailed>(Environment.GetEnvironmentVariable("Kafka:MessageFailedTopic"), $"{Environment.GetEnvironmentVariable("Kafka:MessageFailedTopic")}_group", e =>
                        {
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            e.CreateIfMissing(x => x.ReplicationFactor = 1);
                            e.ConfigureSaga<CounterSagaStateInstance>(context);
                            e.DiscardSkippedMessages();
                        });

                        k.TopicEndpoint<string, MessageCreated>(Environment.GetEnvironmentVariable("Kafka:MessageCreatedTopic"), $"{Environment.GetEnvironmentVariable("Kafka:MessageCreatedTopic")}_group", e =>
                        {
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            e.CreateIfMissing(x => x.ReplicationFactor = 1);
                            e.ConfigureSaga<CounterSagaStateInstance>(context);
                            e.DiscardSkippedMessages();
                        });
                    });
                });

            });

            services.AddHttpClient("messages_client",
                c => c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("Messages:Api")!))
                    .AddHttpMessageHandler(sp => new RequestHeadersMessageHandler(sp));

            services.AddStackExchangeRedisExtensions<MsgPackObjectSerializer>(new RedisConfiguration
            {
                ConnectionString = Environment.GetEnvironmentVariable("Redis:ConnectionString")!
            });

            RedisContext.DefaultSerializer = new MsgPackSerializer();

            return services;
        }
    }
}
