namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a relatoriodestinotipo
/// </summary>
public static class RelatorioDestinoTipoProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar relatoriodestinotipos com paginação
    /// </summary>
    public const string LST_TB_RELATORIO_DESTINO_TIPO = "Program.dbo.LST_TB_RELATORIO_DESTINO_TIPO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um relatoriodestinotipo específico por ID
    /// </summary>
    public const string SNG_TB_RELATORIO_DESTINO_TIPO = "Program.dbo.SNG_TB_RELATORIO_DESTINO_TIPO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_RELATORIO_DESTINO_TIPO = "Program.dbo.APP_TB_RELATORIO_DESTINO_TIPO";
    #endregion
}
