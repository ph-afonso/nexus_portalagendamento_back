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
/// Implementação do serviço de gerenciamento de RelatorioParametroOpcao
/// </summary>
public class RelatorioParametroOpcaoService : IRelatorioParametroOpcaoService
{
    private readonly IRelatorioParametroOpcaoRepository _relatorioparametroopcaoRepository;
    private readonly ILogger<RelatorioParametroOpcaoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioParametroOpcaoService(IRelatorioParametroOpcaoRepository relatorioparametroopcaoRepository, ILogger<RelatorioParametroOpcaoService> logger)
    {
        _relatorioparametroopcaoRepository = relatorioparametroopcaoRepository ?? throw new ArgumentNullException(nameof(relatorioparametroopcaoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioParametroOpcaoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatorioparametroopcaoRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioParametroOpcaoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatorioparametroopcaoRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioParametroOpcaoInputModel>> CreateAsync(RelatorioParametroOpcaoInputModel relatorioparametroopcao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioParametroOpcaoInputModel>();
        try
        {
            relatorioparametroopcao.Operacao = "I"; // Insert

            retorno = await _relatorioparametroopcaoRepository.SalvarAsync(relatorioparametroopcao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioParametroOpcao criado com sucesso. ID: {Id}", retorno.ResultData.CodOpcao);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatorioparametroopcao");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioParametroOpcaoInputModel>> UpdateAsync(RelatorioParametroOpcaoInputModel relatorioparametroopcao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioParametroOpcaoInputModel>();
        try
        {
            relatorioparametroopcao.Operacao = "U"; // Update

            retorno = await _relatorioparametroopcaoRepository.SalvarAsync(relatorioparametroopcao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioParametroOpcao atualizado com sucesso. ID: {Id}", retorno.ResultData.CodOpcao);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatorioparametroopcao");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioParametroOpcaoInputModel>> DeleteAsync(RelatorioParametroOpcaoInputModel relatorioparametroopcao, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioParametroOpcaoInputModel>();
        try
        {
            relatorioparametroopcao.Operacao = "D"; // Delete

            retorno = await _relatorioparametroopcaoRepository.ExcluirAsync(relatorioparametroopcao, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioParametroOpcao excluído com sucesso");
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
