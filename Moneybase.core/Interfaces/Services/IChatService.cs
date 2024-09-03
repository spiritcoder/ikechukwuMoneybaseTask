using System;
using Moneybase.core.Models;

namespace Moneybase.core.Interfaces.Services
{
    public interface IChatService
    {
        Task<ChatSession> CreateChatSessionAsync(ChatSessionRequest chatSessionRequest);
        Task<string> PollSession(string sessionId);
    }
}

