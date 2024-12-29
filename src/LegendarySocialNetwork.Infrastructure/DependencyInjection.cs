using CachingFramework.Redis;
using CachingFramework.Redis.MsgPack;
using Confluent.Kafka;
using LegendarySocialNetwork.Application.Common.Interfaces;
using LegendarySocialNetwork.Application.Consumers;
using LegendarySocialNetwork.Domain.Messages;
using LegendarySocialNetwork.Infrastructure.Caching;
using LegendarySocialNetwork.Infrastructure.Repositories;
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
                x.UsingInMemory();

                x.AddRider(rider =>
                {
                    rider.AddConsumer<UpdateCacheFeedConsumer>();
                    rider.AddConsumer<PushFeedWebSocketConsumer>();


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
                    });
                });

            });

            services.AddStackExchangeRedisExtensions<MsgPackObjectSerializer>(new RedisConfiguration
            {
                ConnectionString = Environment.GetEnvironmentVariable("Redis:ConnectionString")!
            });

            RedisContext.DefaultSerializer = new MsgPackSerializer();

            return services;
        }
    }
}
