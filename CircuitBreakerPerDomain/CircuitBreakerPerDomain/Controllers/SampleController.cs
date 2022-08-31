using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http;

using CircuitBreakerPerDomain.Clients;

using Polly.Timeout;

namespace CircuitBreakerPerDomain.Controllers
{
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        private readonly IResilientClient domain1Client;
        private readonly IResilientClient domain2Client;

        //Receiving two interfaces to be able to get two different named and typed clients
        public SampleController(IHttpClientFactory namedClientFactory, ITypedHttpClientFactory<ResilientClient> namedTypedClientFactory)
        {
            //Retrieving the named clients
            var domain1HttpClient = namedClientFactory.CreateClient("short");
            var domain2HttpClient = namedClientFactory.CreateClient("long");

            //Converting the named clients to typed clients
            domain1Client = namedTypedClientFactory.CreateClient(domain1HttpClient);
            domain2Client = namedTypedClientFactory.CreateClient(domain2HttpClient);
        }

        [HttpGet]
        public async Task<string> Get()
        {
            StringBuilder responseBuilder = new();
            responseBuilder.AppendLine($"Before Domain1 call: {DateTime.UtcNow.ToLongTimeString()}");
            try
            {
                await domain1Client.GetAsync(); //Decorated with CB and timeout policies
            }
            catch (TimeoutRejectedException)
            {
                responseBuilder.AppendLine($"After Domain1 call: {DateTime.UtcNow.ToLongTimeString()}");
            }

            responseBuilder.AppendLine($"Before Domain2 call: {DateTime.UtcNow.ToLongTimeString()}");
            try
            {
                await domain2Client.GetAsync(); //Decorated with CB and timeout policies
            }
            catch (TimeoutRejectedException) 
            {
                //If we would use the same CB instance then we would receive a BrokenCircuitException
                //  See the TRY OUT section inside the Program.cs
                responseBuilder.AppendLine($"After Domain2 call: {DateTime.UtcNow.ToLongTimeString()}");
            }

            return responseBuilder.ToString();
        }
    }
}

