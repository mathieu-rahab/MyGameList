namespace Mygamelist.Hateos;


using Mygamelist.Contracts.Hateos;
public interface IHateosLinkGenerator
{
    Link Generate(string endpointName, object? routeValues, string rel, string method);
}