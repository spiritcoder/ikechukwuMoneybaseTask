using System;
using System.Text.Json;
using Confluent.Kafka;
using Moneybase.core.Interfaces.Services;
using Moneybase.core.Models;

namespace Moneybase.core.Services
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;


        public KafkaProducer(string bootstrapServers, string topic)
        {
            var producerConfig = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<string, string>(producerConfig).Build();


            _topic = topic;
        }

        public async Task EnqueueChatSessionAsync(ChatSession session)
        {
            var message = new Message<string, string>
            {
                Key = session.SessionId,
                Value = JsonSerializer.Serialize(session)
            };

            await _producer.ProduceAsync(_topic, message);
        }

    }
}

