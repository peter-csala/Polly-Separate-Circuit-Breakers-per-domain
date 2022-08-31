using CircuitBreakerPerDomain.Clients;

using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

//Defining a policy to wait only 1 second for the response
//  If that elapsed without receiving it then throw TimeoutRejectedException
IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    => Policy
            .TimeoutAsync<HttpResponseMessage>(1);

//Defining a policy to detect unavailablity/overloadness of a downstream system
//  If a TimeoutRejectedException is seen open the CB for 5 seconds
IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    => Policy<HttpResponseMessage>
            .Handle<TimeoutRejectedException>()
            .CircuitBreakerAsync(1, TimeSpan.FromSeconds(5));

//TRY OUT: Creating a shared CircuitBreaker
//var sharedCB = GetCircuitBreakerPolicy();

//Defining url and name pairs 
//  Each url indicates some delay >> will cause Timeout policy to trigger
var nameUrlPairs = new[] { ("short", "http://httpstat.us/200?sleep=2000"), ("long", "http://httpstat.us/200?sleep=4000") };
foreach (var (name, url) in nameUrlPairs)
{
    //Registering a named and typed client
    builder.Services
        .AddHttpClient<IResilientClient, ResilientClient>(name,
            client => client.BaseAddress = new Uri(url))
        //.AddPolicyHandler(Policy.WrapAsync(sharedCB, GetTimeoutPolicy())); //Using the Circuit Breaker
        .AddPolicyHandler(Policy.WrapAsync(GetCircuitBreakerPolicy(), GetTimeoutPolicy()));
}

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();

