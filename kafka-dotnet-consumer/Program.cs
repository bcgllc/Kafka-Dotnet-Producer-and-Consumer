using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace kafka_dotnet_consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            string brokerList = args[0];
            var topics = args.Skip(1).ToList();

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", brokerList },
                { "group.id", "orders-italy-consumer" }
            };

            using (var consumer = new Consumer<Ignore, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                consumer.Subscribe(topics);
                
                // Raised on critical errors, e.g. connection failures or all brokers down.
                consumer.OnError += (_, error)
                    => Console.WriteLine($"Error: {error}");

                // Raised on deserialization errors or when a consumed message has an error != NoError.
                consumer.OnConsumeError += (_, error)
                    => Console.WriteLine($"Consume error: {error}");

                while (true)
                {
                    Message<Ignore, string> msg;
                    if (consumer.Consume(out msg, TimeSpan.FromSeconds(0.5)))
                    {
                        var committedOffsets = consumer.CommitAsync(msg).Result;
                        Console.WriteLine($"Consuming Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset}");
                    }
                }
            }
        }
    }
}
