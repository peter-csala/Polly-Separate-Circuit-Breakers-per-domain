using System;

namespace CircuitBreakerPerDomain.Clients
{
    //This is just an interface for a typed client
    public interface IResilientClient
    {
        Task GetAsync();
    }
}

