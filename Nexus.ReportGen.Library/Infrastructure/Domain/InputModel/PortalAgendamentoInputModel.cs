using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.Sample.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de Bancos
/// </summary>
public class PortalAgendamentoInputModel : StoredProcedureInputModel
{
    [NexusParameter("IDENTIFICADOR_CLIENTES")]
    public Guid? IdentificadorClientes { get; set; }

    [NexusParameter("DT_SUGESTAO_AGENDA")]
    public DateTime? DtSugestaoAgenda { get; set; }

    [NexusParameter("COD_INCLUSAO_USUARIOS")]
    public int? CodUsuarios { get; set; }

    /// <summary>
    /// Gets or sets the operation identifier associated with the current context.
    /// </summary>
    [NexusParameter("OPERACAO")]
    [Required]
    public string Operacao { get; set; } = "U";

    public PortalAgendamentoInputModel()
    {
        _commandName = "Program.dbo.APP_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO";
    }
}
