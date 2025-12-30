using System.Collections.Generic;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class ConfirmacaoOutputModel
{
    public ValidadeTokenOutputModel? Token { get; set; }
    public List<NotaFiscalOutputModel>? NotasFiscais { get; set; }
    public string? Mensagem { get; set; }

    // --- CORREÇÃO: Adicione esta propriedade ---
    public DateTime? DataSugestao { get; set; }

    public List<AgendamentoDetalheModel> ResultadoProcessamento { get; set; } = new();
}