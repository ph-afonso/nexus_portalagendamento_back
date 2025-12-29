using System;
using System.Collections.Generic;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

public class ConfirmacaoOutputModel
{
    public ValidadeTokenOutputModel Token { get; set; } = new();

    public DateTime? DataSugestaoAgendamento { get; set; }
    public List<NotaFiscalOutputModel> NotasFiscais { get; set; } = new();
    public string Mensagem { get; set; } = string.Empty;
}