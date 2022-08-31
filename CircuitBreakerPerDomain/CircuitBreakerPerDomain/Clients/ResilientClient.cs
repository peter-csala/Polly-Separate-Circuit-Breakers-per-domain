using System;

namespace CircuitBreakerPerDomain.Clients
{
    //This is just a dummy typed client implementation
    public class ResilientClient : IResilientClient
    {
        private readonly HttpClient client;

        public ResilientClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task GetAsync()
        {
            _ = await client.GetAsync("");
        }
    }
    
}

