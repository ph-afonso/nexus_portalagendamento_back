using Microsoft.AspNetCore.Http;
using Nexus.Framework.Data.Attributes;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de Bancos
/// </summary>
public class PortalAgendamentoOutputModel
{
    public DateTime? DataAgendamento { get; set; }
    public List<DateTime> DataAgendamentoList { get; set; }
    public string? DataAgendamentoFormatted { get; set; }
    public DateTime? DataValidade{ get; set; }
    public List<NotaFiscalOutputModel>? NotasFiscais { get; set; }
    public bool TokenValido { get; set; } = true;


}
