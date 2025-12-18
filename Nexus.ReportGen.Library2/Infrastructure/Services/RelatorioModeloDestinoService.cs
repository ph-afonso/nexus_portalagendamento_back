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
/// Implementação do serviço de gerenciamento de RelatorioModeloDestino
/// </summary>
public class RelatorioModeloDestinoService : IRelatorioModeloDestinoService
{
    private readonly IRelatorioModeloDestinoRepository _relatoriomodelodestinoRepository;
    private readonly ILogger<RelatorioModeloDestinoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioModeloDestinoService(IRelatorioModeloDestinoRepository relatoriomodelodestinoRepository, ILogger<RelatorioModeloDestinoService> logger)
    {
        _relatoriomodelodestinoRepository = relatoriomodelodestinoRepository ?? throw new ArgumentNullException(nameof(relatoriomodelodestinoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioModeloDestinoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriomodelodestinoRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioModeloDestinoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriomodelodestinoRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloDestinoInputModel>> CreateAsync(RelatorioModeloDestinoInputModel relatoriomodelodestino, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloDestinoInputModel>();
        try
        {
            relatoriomodelodestino.Operacao = "I"; // Insert
            

            retorno = await _relatoriomodelodestinoRepository.SalvarAsync(relatoriomodelodestino, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloDestino criado com sucesso. ID: {Id}", retorno.ResultData.CodModeloDestino);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriomodelodestino");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloDestinoInputModel>> UpdateAsync(RelatorioModeloDestinoInputModel relatoriomodelodestino, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloDestinoInputModel>();
        try
        {
            relatoriomodelodestino.Operacao = "U"; // Update

            retorno = await _relatoriomodelodestinoRepository.SalvarAsync(relatoriomodelodestino, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloDestino atualizado com sucesso. ID: {Id}", retorno.ResultData.CodModeloDestino);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriomodelodestino");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioModeloDestinoInputModel>> DeleteAsync(RelatorioModeloDestinoInputModel relatoriomodelodestino, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioModeloDestinoInputModel>();
        try
        {
            relatoriomodelodestino.Operacao = "D"; // Delete

            retorno = await _relatoriomodelodestinoRepository.ExcluirAsync(relatoriomodelodestino, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioModeloDestino excluído com sucesso");
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
