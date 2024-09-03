using System;
using Confluent.Kafka;
using Moneybase.core.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Moneybase.core.Services
{

    public interface IKafkaConsumer
    {
        ChatSession ConsumeMessage(string topic);
        Task<ChatSession> GetChatSessionAsync(string sessionId, string topic);
    }

    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly string _bootstrapServers;
        private readonly string _groupId;

        public KafkaConsumer(string bootstrapServers, string groupId)
        {
            _bootstrapServers = bootstrapServers;
            _groupId = groupId;
        }

        public ChatSession ConsumeMessage(string topic)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            try
            {
                var cr = consumer.Consume(TimeSpan.FromSeconds(100)); // Wait for a message with a timeout
                if (cr != null)
                {
                    return DeserializeChatSession(cr.Value);
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occurred: {e.Error.Reason}");
            }
            finally
            {
                consumer.Close();
            }

            return null;
        }


        private ChatSession DeserializeChatSession(string message)
        {
            return JsonSerializer.Deserialize<ChatSession>(message);
        }

        public async Task<ChatSession> GetChatSessionAsync(string sessionId, string topic)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<string, string>(config).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(TimeSpan.FromSeconds(10)); // Adjust timeout as needed

                        if (cr == null || cr.Message == null)
                        {
                            // Timeout or no message found
                            return null;
                        }

                        if (cr.Message.Key.Equals(sessionId, StringComparison.OrdinalIgnoreCase))
                        {
                            return JsonSerializer.Deserialize<ChatSession>(cr.Message.Value);
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    return null;
                }
                finally
                {
                    consumer.Close();
                }
            }
        }


    }
}

