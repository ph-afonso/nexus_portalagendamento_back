namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class AgendamentoDetalheModel
{
    public long NrNota { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Pedido { get; set; } = string.Empty;

    // --- NOVOS CAMPOS ---
    public string NomeFornecedor { get; set; } = string.Empty;
    public string NomeRecebedor { get; set; } = string.Empty;
    // --------------------

    public decimal? Valor { get; set; } // Mantive caso precise no futuro, mas não vou exibir na tabela se não pediu
    public bool Agendado { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string DataAgendada { get; set; } = "-";
    public string HoraAgendada { get; set; } = "-";
}