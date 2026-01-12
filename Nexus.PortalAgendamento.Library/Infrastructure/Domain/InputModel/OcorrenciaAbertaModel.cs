namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

public class OcorrenciaAbertaModel
{
    public int IdOcorrencia { get; set; }

    public int CodOcorrenciaTipo { get; set; }
    public int IdentFilial { get; set; }
    public long NrOcorrencia { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public DateTime? DataAgendamento { get; set; }
    public string HoraAgendamento { get; set; } = string.Empty;

    public int CodFilial { get; set; }
    public int CodConhecimentos { get; set; }
}