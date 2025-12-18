namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriolog
/// </summary>
public static class RelatorioLogProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriologs com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_LOG = "Program.dbo.LST_TB_RELATORIO_LOG";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriolog específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_LOG = "Program.dbo.SNG_TB_RELATORIO_LOG";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_LOG = "Program.dbo.APP_TB_RELATORIO_LOG";
    #endregion
}
