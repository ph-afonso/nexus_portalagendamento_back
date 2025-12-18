namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriosolicitacao
/// </summary>
public static class RelatorioSolicitacaoProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriosolicitacaos com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_SOLICITACAO = "Program.dbo.LST_TB_RELATORIO_SOLICITACAO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriosolicitacao específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_SOLICITACAO = "Program.dbo.SNG_TB_RELATORIO_SOLICITACAO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_SOLICITACAO = "Program.dbo.APP_TB_RELATORIO_SOLICITACAO";
    #endregion
}
