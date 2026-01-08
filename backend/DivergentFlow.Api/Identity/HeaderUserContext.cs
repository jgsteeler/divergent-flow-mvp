using DivergentFlow.Application.Abstractions;

namespace DivergentFlow.Api.Identity;

public sealed class HeaderUserContext : IUserContext
{
    public const string UserIdHeaderName = "X-User-Id";
    public const string DefaultUserId = "local";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HeaderUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                return DefaultUserId;
            }

            var headerValue = httpContext.Request.Headers[UserIdHeaderName].ToString();
            return string.IsNullOrWhiteSpace(headerValue) ? DefaultUserId : headerValue;
        }
    }
}
