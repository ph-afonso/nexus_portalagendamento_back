namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a statusprocessamento
/// </summary>
public static class StatusProcessamentoProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar statusprocessamentos com paginação
    /// </summary>
    public const string LST_TB_STATUS_PROCESSAMENTO = "Program.dbo.LST_TB_STATUS_PROCESSAMENTO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um statusprocessamento específico por ID
    /// </summary>
    public const string SNG_TB_STATUS_PROCESSAMENTO = "Program.dbo.SNG_TB_STATUS_PROCESSAMENTO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_STATUS_PROCESSAMENTO = "Program.dbo.APP_TB_STATUS_PROCESSAMENTO";
    #endregion
}
