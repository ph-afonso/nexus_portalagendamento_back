namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class OcorrenciaAbertaModel
{
    public int CodOcorrenciaTipo { get; set; }     // COD_OCORRENCIAS_TIPO_EJ
    public int IdentFilial { get; set; }           // IDENT_FILIAIS
    public int NrOcorrencia { get; set; }          // NR_OCORRENCIAS
    public string? Observacao { get; set; }        // OBS_OCORRENCIAS
    public DateTime? DataAgendamento { get; set; } // DT_AGENDAMENTO...
    public string? HoraAgendamento { get; set; }   // HR_AGENDAMENTO...

    // Propriedades auxiliares necessárias para o método de Encerramento
    public int CodConhecimentos { get; set; }
    public int CodFilial { get; set; }             // COD_FILIAIS (Interno)
}