namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriomodelodestino
/// </summary>
public static class RelatorioModeloDestinoProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriomodelodestinos com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_MODELO_DESTINO = "Program.dbo.LST_TB_RELATORIO_MODELO_DESTINO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriomodelodestino específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_MODELO_DESTINO = "Program.dbo.SNG_TB_RELATORIO_MODELO_DESTINO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_MODELO_DESTINO = "Program.dbo.APP_TB_RELATORIO_MODELO_DESTINO";
    #endregion
}
