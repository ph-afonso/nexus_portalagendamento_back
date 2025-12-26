using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para atualização de Agendamento
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
    /// Operação a ser realizada (U = Update)
    /// </summary>
    [NexusParameter("OPERACAO")]
    [Required]
    public string Operacao { get; set; } = "U";

    public PortalAgendamentoInputModel()
    {
        // Define a procedure que será chamada pelo Repository automaticamente
        _commandName = "Program.dbo.APP_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO";
    }
}