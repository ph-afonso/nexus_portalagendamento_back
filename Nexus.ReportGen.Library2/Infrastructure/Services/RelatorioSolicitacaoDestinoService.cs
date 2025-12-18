using Microsoft.Extensions.Logging;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain;
using Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;
using Nexus.ReportGen.Library.Infrastructure.Repository.Interfaces;
using Nexus.ReportGen.Library.Infrastructure.Services.Interfaces;

namespace Nexus.ReportGen.Library.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de gerenciamento de RelatorioSolicitacaoDestino
/// </summary>
public class RelatorioSolicitacaoDestinoService : IRelatorioSolicitacaoDestinoService
{
    private readonly IRelatorioSolicitacaoDestinoRepository _relatoriosolicitacaodestinoRepository;
    private readonly ILogger<RelatorioSolicitacaoDestinoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioSolicitacaoDestinoService(IRelatorioSolicitacaoDestinoRepository relatoriosolicitacaodestinoRepository, ILogger<RelatorioSolicitacaoDestinoService> logger)
    {
        _relatoriosolicitacaodestinoRepository = relatoriosolicitacaodestinoRepository ?? throw new ArgumentNullException(nameof(relatoriosolicitacaodestinoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioSolicitacaoDestinoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriosolicitacaodestinoRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioSolicitacaoDestinoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriosolicitacaodestinoRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> CreateAsync(RelatorioSolicitacaoDestinoInputModel relatoriosolicitacaodestino, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioSolicitacaoDestinoInputModel>();
        try
        {
            relatoriosolicitacaodestino.Operacao = "I"; // Insert

            retorno = await _relatoriosolicitacaodestinoRepository.SalvarAsync(relatoriosolicitacaodestino, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioSolicitacaoDestino criado com sucesso. ID: {Id}", retorno.ResultData.CodSolicitacaoDestino);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriosolicitacaodestino");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> UpdateAsync(RelatorioSolicitacaoDestinoInputModel relatoriosolicitacaodestino, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioSolicitacaoDestinoInputModel>();
        try
        {
            relatoriosolicitacaodestino.Operacao = "U"; // Update

            retorno = await _relatoriosolicitacaodestinoRepository.SalvarAsync(relatoriosolicitacaodestino, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioSolicitacaoDestino atualizado com sucesso. ID: {Id}", retorno.ResultData.CodSolicitacaoDestino);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriosolicitacaodestino");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioSolicitacaoDestinoInputModel>> DeleteAsync(RelatorioSolicitacaoDestinoInputModel relatoriosolicitacaodestino, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioSolicitacaoDestinoInputModel>();
        try
        {
            relatoriosolicitacaodestino.Operacao = "D"; // Delete

            retorno = await _relatoriosolicitacaodestinoRepository.ExcluirAsync(relatoriosolicitacaodestino, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioSolicitacaoDestino excluído com sucesso");
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao apagar o registro.");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }
}
