namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class OcorrenciaImpeditivaModel
{
    public int CodOcorrenciaTipo { get; set; } // 140 ou 145
    public int CodConhecimentos { get; set; }  // Necessário para a procedure
    public int CodFilial { get; set; }
}