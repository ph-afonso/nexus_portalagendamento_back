namespace Nexus.PortalAgendamento.MinimalApi.Common;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}