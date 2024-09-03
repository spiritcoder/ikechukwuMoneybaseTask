using System;
namespace Moneybase.core.Models
{
    public class AgentModel
    {
        public string AgentId { get; set; }
        public string Name { get; set; }
        public SeniorityLevel Seniority { get; set; }
        public int MaxConcurrency { get; set; }
        public int CurrentChats { get; set; }
        public bool IsAvailable => CurrentChats < MaxConcurrency && !IsShiftOver();
        public double SeniorityMultiplier => Seniority switch
        {
            SeniorityLevel.Junior => 0.4,
            SeniorityLevel.MidLevel => 0.6,
            SeniorityLevel.Senior => 0.8,
            SeniorityLevel.TeamLead => 0.5,
            _ => 0.4
        };

        public bool IsShiftOver()
        {
            var now = DateTime.UtcNow;
            var shiftStart = now;
            var shiftEnd = shiftStart.AddHours(8);
            return now >= shiftEnd;
        }

        public async Task HandleChat(ChatSession session)
        {
            // Handle the chat session
        }
    }

    public enum SeniorityLevel
    {
        Junior,
        MidLevel,
        Senior,
        TeamLead
    }


    public class AgentRequest
    {
        public string AgentId { get; set; }
        public string Name { get; set; }
        public SeniorityLevel Seniority { get; set; }
        public int MaxConcurrency { get; set; }
    }

}

