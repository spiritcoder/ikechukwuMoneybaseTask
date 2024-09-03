using System;
using Moneybase.core.Interfaces.Services;
using Moneybase.core.Models;

namespace Moneybase.core.Services
{
	public class ChatService: IChatService
	{
        private readonly IKafkaProducer _kafkaProducer;
        private readonly IKafkaConsumer _kafkaConsumer;

        public ChatService(IKafkaProducer kafkaProducer, IKafkaConsumer kafkaConsumer)
        {
            _kafkaProducer = kafkaProducer;
            _kafkaConsumer = kafkaConsumer;
        }

        public async Task<ChatSession> CreateChatSessionAsync(ChatSessionRequest chatSessionRequest)
        {
            var session = new ChatSession
            {
                SessionId = Guid.NewGuid().ToString(),
                Message = chatSessionRequest.Message,
                Email = chatSessionRequest.Email,
                LastPolledAt = DateTime.UtcNow
            };

            await _kafkaProducer.EnqueueChatSessionAsync(session);

            return session;
        }

        public async Task<string> PollSession(string sessionId)
        {
            var session = await _kafkaConsumer.GetChatSessionAsync(sessionId, "chat_sessions");

            if(session == null)
            {
                return ChatSessionStatus.DOESNOTEXIST.ToString();
            }

            //update the last polledAt field
            session.LastPolledAt = DateTime.UtcNow;
            await _kafkaProducer.EnqueueChatSessionAsync(session);
            
            return session.Status.ToString();
            
        }
    }
}

