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
/// Implementação do serviço de gerenciamento de RelatorioSolicitacao
/// </summary>
public class RelatorioSolicitacaoService : IRelatorioSolicitacaoService
{
    private readonly IRelatorioSolicitacaoRepository _relatoriosolicitacaoRepository;
    private readonly ILogger<RelatorioSolicitacaoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioSolicitacaoService(IRelatorioSolicitacaoRepository relatoriosolicitacaoRepository, ILogger<RelatorioSolicitacaoService> logger)
    {
        _relatoriosolicitacaoRepository = relatoriosolicitacaoRepository ?? throw new ArgumentNullException(nameof(relatoriosolicitacaoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioSolicitacaoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriosolicitacaoRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioSolicitacaoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriosolicitacaoRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioSolicitacaoInputModel>> CreateAsync(RelatorioSolicitacaoInputModel relatoriosolicitacao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioSolicitacaoInputModel>();
        try
        {
            relatoriosolicitacao.Operacao = "I"; // Insert

            retorno = await _relatoriosolicitacaoRepository.SalvarAsync(relatoriosolicitacao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioSolicitacao criado com sucesso. ID: {Id}", retorno.ResultData.CodRelatorioSolicitacao);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriosolicitacao");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioSolicitacaoInputModel>> UpdateAsync(RelatorioSolicitacaoInputModel relatoriosolicitacao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioSolicitacaoInputModel>();
        try
        {
            relatoriosolicitacao.Operacao = "U"; // Update

            retorno = await _relatoriosolicitacaoRepository.SalvarAsync(relatoriosolicitacao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioSolicitacao atualizado com sucesso. ID: {Id}", retorno.ResultData.CodRelatorioSolicitacao);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriosolicitacao");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioSolicitacaoInputModel>> DeleteAsync(RelatorioSolicitacaoInputModel relatoriosolicitacao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioSolicitacaoInputModel>();
        try
        {
            relatoriosolicitacao.Operacao = "D"; // Delete

            retorno = await _relatoriosolicitacaoRepository.ExcluirAsync(relatoriosolicitacao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioSolicitacao excluído com sucesso");
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
