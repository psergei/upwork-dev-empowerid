using System.Security.Claims;

namespace Gateway.Services;

public class BlogUserService: IBlogUserService
{
  private readonly IHttpContextAccessor _httpCtxAccessor;

  public BlogUserService(IHttpContextAccessor ctxAccessor)
  {
    _httpCtxAccessor = ctxAccessor;
  }

  public string UserName
  {
    get
    {
      var user = _httpCtxAccessor.HttpContext?.User;

      return user?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    }
  }
}