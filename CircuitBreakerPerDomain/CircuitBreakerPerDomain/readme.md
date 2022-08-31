# Polly usage example - Separate Circuit Breakers per domain
This sample project was put together based on the following [SO question](https://stackoverflow.com/questions/58438204/proper-way-to-handle-multiple-services-with-polly-circuit-breaker/).
## Problem (in short)
- How to define separate circuit breaker policies for different domains?
- NOTE: For a single domain use the Circuit Breaker as a shared policy.
## Design
### Option A
Use `IDictionary` or `IConcurrentPolicyRegistry` collections to store policies per domain
#### Pro
The implementation is simple,  you have to populate a collection and look up the appropriate policy whenever needed
#### Con
You have to explicitly call the retrieved policy's `ExecuteAsync` 
### Option B
Use named typed clients and decorate them with Circuit Breakers
#### Pro
The policy usage is done implicitly on your behalf
#### Con
The setup part is a bit more complex compared to *Option A*

## Solution
This sample project shows how to implement *Option B*
- `Clients` folder contains the definition of the typed client (interface + dummy implementation)
- `Program.cs` contains the policy definitions and the registration of the named and typed clients
- `SampleController.cs` shows the usage of the named and typed clients
