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
/// Implementação do serviço de gerenciamento de RelatorioModelo
/// </summary>
public class RelatorioModeloService : IRelatorioModeloService
{
    private readonly IRelatorioModeloRepository _relatoriomodeloRepository;
    private readonly ILogger<RelatorioModeloService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioModeloService(IRelatorioModeloRepository relatoriomodeloRepository, ILogger<RelatorioModeloService> logger)
    {
        _relatoriomodeloRepository = relatoriomodeloRepository ?? throw new ArgumentNullException(nameof(relatoriomodeloRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioModeloOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriomodeloRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioModeloOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriomodeloRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloInputModel>> CreateAsync(RelatorioModeloInputModel relatoriomodelo, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloInputModel>();
        try
        {
            relatoriomodelo.Operacao = "I"; // Insert

            retorno = await _relatoriomodeloRepository.SalvarAsync(relatoriomodelo, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModelo criado com sucesso. ID: {Id}", retorno.ResultData.CodRelatorioModelo);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriomodelo");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloInputModel>> UpdateAsync(RelatorioModeloInputModel relatoriomodelo, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloInputModel>();
        try
        {
            relatoriomodelo.Operacao = "U"; // Update

            retorno = await _relatoriomodeloRepository.SalvarAsync(relatoriomodelo, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModelo atualizado com sucesso. ID: {Id}", retorno.ResultData.CodRelatorioModelo);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriomodelo");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloInputModel>> DeleteAsync(RelatorioModeloInputModel relatoriomodelo, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloInputModel>();
        try
        {
            relatoriomodelo.Operacao = "D"; // Delete

            retorno = await _relatoriomodeloRepository.ExcluirAsync(relatoriomodelo, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModelo excluído com sucesso");
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
