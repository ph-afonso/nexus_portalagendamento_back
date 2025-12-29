namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class ValidadeTokenOutputModel
{
    // --- STATUS ---
    public bool TokenValido { get; set; }
    public DateTime? DataGeracaoToken { get; set; }
    public DateTime? DataAtualizacaoToken { get; set; }
    public DateTime? DataSugestaoAgendamento { get; set; }
    public DateTime? DataBaseCalculo => DataAtualizacaoToken ?? DataGeracaoToken;
    public DateTime? DataExpiracaoToken => DataBaseCalculo?.AddHours(48);
}