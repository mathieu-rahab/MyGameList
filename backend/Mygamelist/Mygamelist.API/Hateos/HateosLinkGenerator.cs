namespace Mygamelist.Hateos;

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
        return new Link
        {
            Href = _linkGenerator.GetUriByName(
                _httpContextAccessor.HttpContext,
                endpointName,
                routeValues),
            Method = method,
            Rel = rel
        };
    }
}