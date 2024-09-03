using System;
using Moneybase.core.Models;

namespace Moneybase.core.Interfaces.Services
{
	public interface IKafkaProducer
    {
		public Task EnqueueChatSessionAsync(ChatSession chatSession);
    }
}

