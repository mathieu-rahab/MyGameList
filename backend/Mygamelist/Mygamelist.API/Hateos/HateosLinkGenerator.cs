namespace Mygamelist.Hateos;

using Mygamelist.Contracts.Hateos;
public class HateosLinkGenerator : IHateosLinkGenerator
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HateosLinkGenerator(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public Link Generate(string endpointName, object? routeValues, string rel, string method)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new InvalidOperationException("HttpContext is not available. Ensure this method is called within a request context.");
        }
    
        var href = _linkGenerator.GetUriByName(
            httpContext,
            endpointName,
            routeValues);
    
        if (string.IsNullOrEmpty(href))
        {
            throw new InvalidOperationException($"Could not generate URI for endpoint '{endpointName}'");
        }
    
        return new Link
        {
            Href = href,
            Method = method,
            Rel = rel
        };
    }
}