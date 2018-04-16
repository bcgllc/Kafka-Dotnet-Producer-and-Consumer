using System;
using System.Text;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace kafka_dotnet_producer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: .. brokerList topicName");
                return;
            }

            string brokerList = args[0];
            string topicName = args[1];

            var config = new Dictionary<string, object> {
                { "bootstrap.servers", brokerList },
                { "api.version.request", "false" }               
            };

            KafkaProducer.Config = config;
            KafkaProducer.TopicName = topicName;

#if DEBUG
            string watchPath = "./data";
            //Console.WriteLine(String.Format("Watching: {0}", watchPath));
#elif RELEASE
            string watchPath = "../data";
#endif

            FileWatcher fileWatcher = new FileWatcher(watchPath);

            // wait - not to end
            new System.Threading.AutoResetEvent(false).WaitOne();
        }
    }
}
