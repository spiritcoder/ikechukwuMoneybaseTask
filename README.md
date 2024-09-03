
The entire solution consists of a SharedProjects folder that contains all interfaces, models and services that is shared by the different applications in the solution

The repository is made up of two APPS
1. A chatAPI - this is a dotnetcore API that handles chat session creation, and polling with the two endpoints. It publishes to a kafka topic and also fetches already
    published messages using their session ID.

2. AgentManagementService - This service is a dotnet core WorkerService. It is in charge of assigning sessions to agents. There is a predefined agent list in the program.cs
    which can be changed. This worker service is a background service that also marks sessions as inactive, consumes from the kafka topic.


 The environmentvariables are set in the launchSettings.json file for each of the projects.

 The frontend is to consume the two APIs on the chatAPI but I couldn't implement it because of some tight schedules.


 You can run the projects as you run normal c# projects on Visual studio.

 I added a docker-compose.yml file for setting up and bringing up the kafka service and zookeeper on docker.

 I can add the DockerFile for the two APPS but out of time. Had some tight schedules at my current workplace.