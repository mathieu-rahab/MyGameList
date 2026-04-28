namespace Mygamelist.Hateos;

public interface IHateosLinkGenerator
{
    Link Generate(string endpointName, object? routeValues, string rel, string method);
}