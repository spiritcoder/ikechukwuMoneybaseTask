using AgentManagementService;
using Moneybase.core;
using Moneybase.core.Interfaces.Services;
using Moneybase.core.Models;
using Moneybase.core.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        // Register other services as needed
        services.AddSingleton<IKafkaConsumer>(sp =>
        {
            return new KafkaConsumer(EnvironmentVariables.KafkaURL, EnvironmentVariables.KafkaGroupID);
        });

        services.AddSingleton<IKafkaProducer>(sp =>
        {
            return new KafkaProducer(EnvironmentVariables.KafkaURL, EnvironmentVariables.KafkaTopic);
        });
        services.AddSingleton(new List<AgentModel>
        {
            new AgentModel { AgentId = "A1", Name = "Agent 1", Seniority = SeniorityLevel.TeamLead, MaxConcurrency = 8 },
            new AgentModel { AgentId = "A2", Name = "Agent 2", Seniority = SeniorityLevel.MidLevel, MaxConcurrency = 6 },
            new AgentModel { AgentId = "A3", Name = "Agent 3", Seniority = SeniorityLevel.MidLevel, MaxConcurrency = 6 },
            new AgentModel { AgentId = "A4", Name = "Agent 4", Seniority = SeniorityLevel.Junior, MaxConcurrency = 4 },
            // Add more agents
        });
    })
    .Build();

host.Run();

