using Confluent.Kafka;
using LegendarySocialNetwork.Domain.Messages;
using LegendarySocialNetwork.Messages.Consumers;
using MassTransit;

namespace LegendarySocialNetwork.Messages
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastracture(this IServiceCollection services)
        {
            services.AddScoped<CreateMessageConsumer>();

            services.AddMassTransit(x =>
            {
                x.UsingInMemory();

                x.AddRider(rider =>
                {
                    rider.AddConsumer<CreateMessageConsumer>();
                    rider.AddProducer<string, MessageCreated>(Environment.GetEnvironmentVariable("Kafka:MessageCreatedTopic"));
                    rider.AddProducer<string, MessageFailed>(Environment.GetEnvironmentVariable("Kafka:MessageFailedTopic"));

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host(Environment.GetEnvironmentVariable("Kafka:Host"));

                        k.TopicEndpoint<CreateMessage>(Environment.GetEnvironmentVariable("Kafka:CreateMessageTopic"),
                            Environment.GetEnvironmentVariable("Kafka:CreateMessageGroup"), e =>
                            {
                                e.CreateIfMissing(x => x.ReplicationFactor = 1);
                                e.ConfigureConsumer<CreateMessageConsumer>(context);
                                e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            });
                    });
                });
            });
            return services;
        }
    }
}
