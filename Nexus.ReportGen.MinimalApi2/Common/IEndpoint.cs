namespace Nexus.ReportGen.MinimalApi.Common;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}