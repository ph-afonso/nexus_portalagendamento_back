namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriomodelopermissao
/// </summary>
public static class RelatorioModeloPermissaoProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriomodelopermissaos com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_MODELO_PERMISSAO = "Program.dbo.LST_TB_RELATORIO_MODELO_PERMISSAO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriomodelopermissao específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_MODELO_PERMISSAO = "Program.dbo.SNG_TB_RELATORIO_MODELO_PERMISSAO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_MODELO_PERMISSAO = "Program.dbo.APP_TB_RELATORIO_MODELO_PERMISSAO";
    #endregion
}
