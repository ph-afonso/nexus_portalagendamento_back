namespace Nexus.ReportGen.Library.Infrastructure.Constants;

/// <summary>
/// Constantes para procedures relacionadas a tipoparametro
/// </summary>
public static class TipoParametroProcedures
{
    #region List Procedures
    /// <summary>
    /// Procedure para listar tipoparametros com paginação
    /// </summary>
    public const string LST_TB_TIPO_PARAMETRO = "Program.dbo.LST_TB_TIPO_PARAMETRO";

    #endregion

    #region Get Procedures
    /// <summary>
    /// Procedure para buscar um tipoparametro específico por ID
    /// </summary>
    public const string SNG_TB_TIPO_PARAMETRO = "Program.dbo.SNG_TB_TIPO_PARAMETRO";
    #endregion

    #region App Procedures
    /// <summary>
    /// Procedure para operações de CRUD (Create, Read, Update, Delete)
    /// </summary>
    public const string APP_TB_TIPO_PARAMETRO = "Program.dbo.APP_TB_TIPO_PARAMETRO";
    #endregion
}
