namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatorioparametroopcao
/// </summary>
public static class RelatorioParametroOpcaoProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatorioparametroopcaos com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_PARAMETRO_OPCAO = "Program.dbo.LST_TB_RELATORIO_PARAMETRO_OPCAO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatorioparametroopcao específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_PARAMETRO_OPCAO = "Program.dbo.SNG_TB_RELATORIO_PARAMETRO_OPCAO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_PARAMETRO_OPCAO = "Program.dbo.APP_TB_RELATORIO_PARAMETRO_OPCAO";
    #endregion
}
