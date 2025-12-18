namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriocategoria
/// </summary>
public static class RelatorioCategoriaProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriocategorias com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_CATEGORIA = "Program.dbo.LST_TB_RELATORIO_CATEGORIA";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriocategoria específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_CATEGORIA = "Program.dbo.SNG_TB_RELATORIO_CATEGORIA";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_CATEGORIA = "Program.dbo.APP_TB_RELATORIO_CATEGORIA";
    #endregion
}
