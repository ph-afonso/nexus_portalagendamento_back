namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriosolicitacaodestino
/// </summary>
public static class RelatorioSolicitacaoDestinoProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriosolicitacaodestinos com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_SOLICITACAO_DESTINO = "Program.dbo.LST_TB_RELATORIO_SOLICITACAO_DESTINO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriosolicitacaodestino específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_SOLICITACAO_DESTINO = "Program.dbo.SNG_TB_RELATORIO_SOLICITACAO_DESTINO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_SOLICITACAO_DESTINO = "Program.dbo.APP_TB_RELATORIO_SOLICITACAO_DESTINO";
    #endregion
}
