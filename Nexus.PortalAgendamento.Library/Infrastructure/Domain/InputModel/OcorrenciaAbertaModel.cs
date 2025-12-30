namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

public class OcorrenciaAbertaModel
{
    // Identificador único (Primary Key da tabela de Ocorrências) - ADICIONADO
    public int IdOcorrencia { get; set; }

    public int CodOcorrenciaTipo { get; set; }
    public int IdentFilial { get; set; }
    public long NrOcorrencia { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public DateTime? DataAgendamento { get; set; }
    public string HoraAgendamento { get; set; } = string.Empty;

    // Dados para Encerramento
    public int CodFilial { get; set; }
    public int CodConhecimentos { get; set; }
}