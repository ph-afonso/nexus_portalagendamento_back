namespace Nexus.Sample.MinimalApi.Common;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}