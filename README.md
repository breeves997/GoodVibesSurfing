# GoodVibesSurfing

Good Vibes Surfing is a Microsoft Service Fabric sample application geared towards learning MicroSOA through experimentation and tinkering. The intent of this project is to allow developers to develop and interact with microservices and cloud programming patterns in a safe and worry-free environment.

Every piece of functionality is expected to be heavily commented and documented, with the intent that a developer new to this project and Service Fabric will be able to follow along to what is happening in code. Naturally, some code will inevitably exist that necessitates some background information and experiencec, but the objective is a "best effort" at understandability. 

An ASP .NET Core web front end is exposed as a means to interact with and test the various services and functionality of the system. It is expected that a description of the "how and why" of the server code is added to the html of each page, with references to specific files and resources an individual would need to step through and understand what is happening on the back end.

## Requirements
 - [DotNet Core](https://www.asp.net/core)
 - [Service Fabric](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started)
 

## Todos

### Code
 - Write test framework + tests. Use the [Service Fabric Mocks!](https://github.com/loekd/ServiceFabric.Mocks)
 - Figure out why the blob SAS key generated from the ValetAccessManager is getting auth failures from azure
 - Create a front end framework. Preference to Angular2, but others considered
 - Create sample Actors project
 
### DevOps
 - Create some external data stores (SQL, NoSQL) to interact with
 - Deploy live version to fabric party clusters
 - Get stateful service replicas up and running
