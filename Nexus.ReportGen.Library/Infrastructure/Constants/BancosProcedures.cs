namespace Nexus.Sample.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a bancos
/// </summary>
public static class BancosProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar bancoss com paginação
    /// </summary>
    public const string LST_TB_BANCOS = "Program.dbo.LST_TB_BANCOS";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um bancos específico por ID
    /// </summary>
    public const string SNG_TB_BANCOS = "Program.dbo.SNG_TB_BANCOS";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_BANCOS = "Program.dbo.APP_TB_BANCOS";
    #endregion
}
