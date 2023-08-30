
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Gateway.FunctionalTests;

internal class FakePolicyEvaluator : IPolicyEvaluator
{
  public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
  {
    var principal = new ClaimsPrincipal();

    var schemeName = "FakeScheme";
    principal.AddIdentity(new ClaimsIdentity(new []
    {
      new Claim("name", "integration")
    }, schemeName));

    var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), schemeName);

    return await Task.FromResult(AuthenticateResult.Success(ticket));
  }

  public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
  {
    return await Task.FromResult(PolicyAuthorizationResult.Success());
  }
}