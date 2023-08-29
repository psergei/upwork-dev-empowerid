using Gateway.Clients;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.FunctionalTests;

public static class TestClient
{
  public static HttpClient GetTestClient()
  {
    return new WebApplicationFactory<BlogApiClient>().WithWebHostBuilder(builder =>
    {
      builder.ConfigureTestServices(services =>
      {
        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
      });
    }).CreateClient();
  }
}

