using System;
namespace Moneybase.core.Models
{
    public class ChatSession : ChatSessionRequest
    {
        public string SessionId { get; set; }
        public string AgentId { get; set; }
        public DateTime LastPolledAt { get; set; }
        public ChatSessionStatus Status { get; set; }
    }

    public class ChatSessionRequest
    {
        public string Message { get; set; }
        public string Email { get; set; }
    }
    public enum ChatSessionStatus
    {
        Pending,
        Assigned,
        Inactive,
        DOESNOTEXIST
    }

}

