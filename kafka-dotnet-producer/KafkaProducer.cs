/**

dotnet publish -c Release -o dist

for i in {1..50}; do cp italy-order-01.xml italy-order-copy-$i.xml; done

 */

using System;
using System.Text;
using System.Collections.Generic;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

public static class KafkaProducer {

    public static Dictionary<string, object> Config;
    public static string TopicName = String.Empty;

    public static void Produce(string jsonObject) {
        using (var producer = new Producer<Null, string>(Config, null, new StringSerializer(Encoding.UTF8)))
        {
            var deliveryReport = producer.ProduceAsync(TopicName, null, jsonObject);
            deliveryReport.ContinueWith(task =>
            {
                if (task.Exception != null) {
                    Console.WriteLine(task.Exception.InnerException.Message);
                }
                Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
            });

            // Tasks are not waited on synchronously (ContinueWith is not synchronous),
            // so it's possible they may still in progress here.
            producer.Flush(TimeSpan.FromSeconds(10));
        }
    }

    public static void Produce(string topicName, string jsonObject) {
        TopicName = topicName;
        Produce(jsonObject);
    }
}