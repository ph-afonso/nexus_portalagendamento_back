using Microsoft.Extensions.Logging;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Services;

public class PortalAgendamentoService : IPortalAgendamentoService
{
    private readonly IPortalAgendamentoRepository _repository;
    private readonly ILogger<PortalAgendamentoService> _logger;

    public PortalAgendamentoService(
        IPortalAgendamentoRepository repository,
        ILogger<PortalAgendamentoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // --- ROTA 1: CONFIRMAR ---
    public async Task<NexusResult<bool>> Confirmar(ConfirmarAgendamentoInputModel model, CancellationToken ct)
    {
        if (!await ValidarTokenInterno(model.IdentificadorCliente, ct))
            return RetornarErroExpirado();

        return await _repository.ConfirmarAgendamento(model, ct);
    }

    // --- ROTA 2: SOLICITAR NOVA DATA ---
    public async Task<NexusResult<bool>> SolicitarNovaData(SolicitarNovaDataInputModel model, CancellationToken ct)
    {
        if (!await ValidarTokenInterno(model.IdentificadorCliente, ct))
            return RetornarErroExpirado();

        return await _repository.SolicitarNovaData(model, ct);
    }

    // --- ROTA 3: POSSUO ANEXO ---
    public async Task<NexusResult<bool>> EnviarAnexo(EnviarAnexoInputModel model, CancellationToken ct)
    {
        if (!await ValidarTokenInterno(model.IdentificadorCliente, ct))
            return RetornarErroExpirado();

        if (model.Arquivo == null || model.Arquivo.Length == 0)
        {
            var res = new NexusResult<bool>();
            res.AddFailureMessage("Arquivo inválido.");
            return res;
        }

        // Define onde salvar (ajuste conforme seu servidor)
        var tempPath = Path.GetTempPath();
        var fileName = $"{model.IdentificadorCliente}_{Guid.NewGuid()}_{model.Arquivo.FileName}";
        var fullPath = Path.Combine(tempPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await model.Arquivo.CopyToAsync(stream, ct);
        }

        return await _repository.RegistrarAnexo(model.IdentificadorCliente, fullPath, ct);
    }

    // --- MÉTODOS PRIVADOS (Blindagem) ---

    private async Task<bool> ValidarTokenInterno(Guid guid, CancellationToken ct)
    {
        var result = await _repository.GetDadosValidacaoToken(guid, ct);

        // Se não achou no banco ou deu erro
        if (!result.IsSuccess || result.ResultData == null) return false;

        // Dynamic para facilitar a leitura das colunas SQL
        dynamic dados = result.ResultData;

        // Se tiver alteração, usa ela. Se não, usa inclusão.
        // Cuidado com Nulos vindo do Dapper dynamic
        DateTime? dtInc = dados.DT_INCLUSAO;
        DateTime? dtAlt = dados.DT_ALTERACAO;

        DateTime dataBase = dtAlt ?? dtInc ?? DateTime.MinValue;

        if (dataBase == DateTime.MinValue) return false;

        // Regra de 48 horas
        if (DateTime.Now > dataBase.AddHours(48)) return false;

        return true;
    }

    private NexusResult<bool> RetornarErroExpirado()
    {
        var res = new NexusResult<bool>();
        res.AddFailureMessage("O link de agendamento expirou.");
        return res;
    }
}