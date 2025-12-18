namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriomodelo
/// </summary>
public static class RelatorioModeloProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriomodelos com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_MODELO = "Program.dbo.LST_TB_RELATORIO_MODELO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriomodelo específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_MODELO = "Program.dbo.SNG_TB_RELATORIO_MODELO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_MODELO = "Program.dbo.APP_TB_RELATORIO_MODELO";
    #endregion
}
