using Nexus.ReportGen.MinimalApi.Endpoints.RelatorioCategoria;

namespace Nexus.ReportGen.MinimalApi.Endpoints;

/// <summary>
/// Agrupador de endpoints relacionados a Bancos
/// </summary>

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { message = "OK" });

        // Registrar os 4 endpoints principais
        CreateRelatorioCategoriaEndpoint.Map(endpoints);
        GetAllRelatorioCategoriaEndpoint.Map(endpoints);
        GetRelatorioCategoriaByIdEndpoint.Map(endpoints);

        RelatorioDestinoTipo.CreateRelatorioDestinoTipoEndpoint.Map(endpoints);
        RelatorioDestinoTipo.GetAllRelatorioDestinoTipoEndpoint.Map(endpoints);
        RelatorioDestinoTipo.GetRelatorioDestinoTipoByIdEndpoint.Map(endpoints);

        RelatorioLog.CreateRelatorioLogEndpoint.Map(endpoints);
        RelatorioLog.GetAllRelatorioLogEndpoint.Map(endpoints);
        RelatorioLog.GetRelatorioLogByIdEndpoint.Map(endpoints);

        RelatorioModelo.CreateRelatorioModeloEndpoint.Map(endpoints);
        RelatorioModelo.GetAllRelatorioModeloEndpoint.Map(endpoints);
        RelatorioModelo.GetRelatorioModeloByIdEndpoint.Map(endpoints);

        RelatorioModeloDestino.CreateRelatorioModeloDestinoEndpoint.Map(endpoints);
        RelatorioModeloDestino.GetAllRelatorioModeloDestinoEndpoint.Map(endpoints);
        RelatorioModeloDestino.GetRelatorioModeloDestinoByIdEndpoint.Map(endpoints);

        RelatorioModeloParametro.CreateRelatorioModeloParametroEndpoint.Map(endpoints);
        RelatorioModeloParametro.GetAllRelatorioModeloParametroEndpoint.Map(endpoints);
        RelatorioModeloParametro.GetRelatorioModeloParametroByIdEndpoint.Map(endpoints);

        RelatorioModeloPermissao.CreateRelatorioModeloPermissaoEndpoint.Map(endpoints);
        RelatorioModeloPermissao.GetAllRelatorioModeloPermissaoEndpoint.Map(endpoints);
        RelatorioModeloPermissao.GetRelatorioModeloPermissaoByIdEndpoint.Map(endpoints);

        RelatorioModeloSaida.CreateRelatorioModeloSaidaEndpoint.Map(endpoints);
        RelatorioModeloSaida.GetAllRelatorioModeloSaidaEndpoint.Map(endpoints);
        RelatorioModeloSaida.GetRelatorioModeloSaidaByIdEndpoint.Map(endpoints);

        RelatorioParametroOpcao.CreateRelatorioParametroOpcaoEndpoint.Map(endpoints);
        RelatorioParametroOpcao.GetAllRelatorioParametroOpcaoEndpoint.Map(endpoints);
        RelatorioParametroOpcao.GetRelatorioParametroOpcaoByIdEndpoint.Map(endpoints);

        RelatorioSolicitacao.CreateRelatorioSolicitacaoEndpoint.Map(endpoints);
        RelatorioSolicitacao.GetAllRelatorioSolicitacaoEndpoint.Map(endpoints);
        RelatorioSolicitacao.GetRelatorioSolicitacaoByIdEndpoint.Map(endpoints);

        RelatorioSolicitacaoDestino.CreateRelatorioSolicitacaoDestinoEndpoint.Map(endpoints);
        RelatorioSolicitacaoDestino.GetAllRelatorioSolicitacaoDestinoEndpoint.Map(endpoints);
        RelatorioSolicitacaoDestino.GetRelatorioSolicitacaoDestinoByIdEndpoint.Map(endpoints);

        StatusProcessamento.CreateStatusProcessamentoEndpoint.Map(endpoints);
        StatusProcessamento.GetAllStatusProcessamentoEndpoint.Map(endpoints);
        StatusProcessamento.GetStatusProcessamentoByIdEndpoint.Map(endpoints);

        TipoParametro.CreateTipoParametroEndpoint.Map(endpoints);
        TipoParametro.GetAllTipoParametroEndpoint.Map(endpoints);
        TipoParametro.GetTipoParametroByIdEndpoint.Map(endpoints);

    }

}