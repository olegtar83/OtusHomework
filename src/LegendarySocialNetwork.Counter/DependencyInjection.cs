using Confluent.Kafka;
using LegendarySocialNetwork.Counter.Consumers;
using LegendarySocialNetwork.Domain.Messages;
using MassTransit;


namespace LegendarySocialNetwork.Counter
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastracture(this IServiceCollection services)
        {
            services.AddScoped<CounterConsumer>();

            services.AddMassTransit(x =>
            {
                x.UsingInMemory();

                x.AddRider(rider =>
                {
                    rider.AddConsumer<CounterConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host(Environment.GetEnvironmentVariable("Kafka:Host"));

                        k.TopicEndpoint<IncrementCounter>(Environment.GetEnvironmentVariable("Kafka:IncrementCounterTopic"),
                            Environment.GetEnvironmentVariable("Kafka:IncrementCounterGroup"), e =>
                            {
                                e.CreateIfMissing(x => x.ReplicationFactor = 1);
                                e.ConfigureConsumer<CounterConsumer>(context);
                                e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            });

                        k.TopicEndpoint<DecrementCounter>(Environment.GetEnvironmentVariable("Kafka:DecrementCounterTopic"),
                           Environment.GetEnvironmentVariable("Kafka:DecrementCounterGroup"), e =>
                           {
                               e.CreateIfMissing(x => x.ReplicationFactor = 1);
                               e.ConfigureConsumer<CounterConsumer>(context);
                               e.AutoOffsetReset = AutoOffsetReset.Earliest;
                           });
                    });
                });
            });

            return services;
        }
    }
}
