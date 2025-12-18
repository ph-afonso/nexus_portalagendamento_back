namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriomodelosaida
/// </summary>
public static class RelatorioModeloSaidaProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriomodelosaidas com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_MODELO_SAIDA = "Program.dbo.LST_TB_RELATORIO_MODELO_SAIDA";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriomodelosaida específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_MODELO_SAIDA = "Program.dbo.SNG_TB_RELATORIO_MODELO_SAIDA";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_MODELO_SAIDA = "Program.dbo.APP_TB_RELATORIO_MODELO_SAIDA";
    #endregion
}
