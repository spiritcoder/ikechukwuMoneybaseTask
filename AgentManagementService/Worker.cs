using Moneybase.core.Interfaces.Services;
using Moneybase.core.Models;
using Moneybase.core.Services;
using Newtonsoft.Json;

namespace AgentManagementService;

public class Worker : BackgroundService
{
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly List<AgentModel> _agents;
    private readonly List<AgentModel> _overflowTeam;
    private readonly int _maxQueueLength;
    private const int PollingInterval = 1000; // 1 second
    private const int InactivityThreshold = 3; // 3 missed polls
    private readonly ILogger<Worker> _logger;


    public Worker(IKafkaConsumer kafkaConsumer, List<AgentModel> agents, List<AgentModel> overflowTeam, ILogger<Worker> logger, IKafkaProducer kafkaProducer)
    {
        _kafkaConsumer = kafkaConsumer;
        _kafkaProducer = kafkaProducer;
        _agents = agents;
        _overflowTeam = overflowTeam;
        _logger = logger;

        int totalCapacity = CalculateTeamCapacity(_agents);
        _maxQueueLength = (int)(totalCapacity * 1.5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       try
        {
            _logger.LogInformation("AgentAssignmentService started");
            while (!stoppingToken.IsCancellationRequested)
            {
                var chatSession = _kafkaConsumer.ConsumeMessage("chat_sessions");
                if (chatSession != null)
                {
                    if (IsSessionInactive(chatSession))
                    {
                        MarkSessionInactive(chatSession);
                        _logger.LogInformation("Session marked as inactive");

                    }
                    else
                    {
                        await AssignChatToAgent(chatSession);
                        _logger.LogInformation("Session assigned to agent");

                    }
                }
                _logger.LogInformation("No Session found");

                await Task.Delay(PollingInterval, stoppingToken);
            }
        } catch(Exception ex)
        {
            _logger.LogInformation(ex, "An error occurred in the AgentAssignmentService.");
        }
    }

    private bool IsSessionInactive(ChatSession session)
    {
        return (DateTime.UtcNow - session.LastPolledAt).TotalSeconds > PollingInterval * InactivityThreshold;
    }

    private void MarkSessionInactive(ChatSession session)
    {
        session.Status = ChatSessionStatus.Inactive;
        _kafkaProducer.EnqueueChatSessionAsync(session);
        _logger.LogInformation($"Session {session.SessionId} marked as inactive.");
    }

    private async Task AssignChatToAgent(ChatSession session)
    {
        _logger.LogInformation($"Session: {JsonConvert.SerializeObject(session)}");
        var availableAgent = GetAvailableAgent();

        _logger.LogInformation($"Available Agents: {JsonConvert.SerializeObject(availableAgent)}");

        if (availableAgent != null)
        {

            availableAgent.CurrentChats++;
            session.AgentId = availableAgent.AgentId;
            session.Status = ChatSessionStatus.Assigned;
            _logger.LogInformation($"Session {session.SessionId} assigned to agent {availableAgent.Name}");
        }
        else
        {
            _logger.LogInformation($"No available agent for session {session.SessionId}. Queue might be full.");
        }

        await Task.CompletedTask;
    }

    private AgentModel GetAvailableAgent()
    {
        // Assign chats in a round-robin fashion, preferring juniors first
        foreach (var agent in _agents.Where(a => a.IsAvailable))
        {
            if (agent.CurrentChats < agent.MaxConcurrency)
            {
                return agent;
            }
        }

        // If no regular agent is available, check if overflow is needed
        if (IsOverflowNeeded())
        {
            foreach (var agent in _overflowTeam.Where(a => a.IsAvailable))
            {
                if (agent.CurrentChats < agent.MaxConcurrency)
                {
                    return agent;
                }
            }
        }

        return null;
    }

    private bool IsOverflowNeeded()
    {
        return DateTime.UtcNow.Hour >= 9 && DateTime.UtcNow.Hour < 17 &&
               _agents.Sum(a => a.CurrentChats) >= _maxQueueLength;
    }

    private int CalculateTeamCapacity(List<AgentModel> team)
    {
        return team.Sum(agent => (int)(agent.MaxConcurrency * agent.SeniorityMultiplier));
    }

}

