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
/// Implementação do serviço de gerenciamento de RelatorioDestinoTipo
/// </summary>
public class RelatorioDestinoTipoService : IRelatorioDestinoTipoService
{
    private readonly IRelatorioDestinoTipoRepository _relatoriodestinotipoRepository;
    private readonly ILogger<RelatorioDestinoTipoService> _logger;

    /// <summary>
    /// Construtor
    /// </summary>
    public RelatorioDestinoTipoService(IRelatorioDestinoTipoRepository relatoriodestinotipoRepository, ILogger<RelatorioDestinoTipoService> logger)
    {
        _relatoriodestinotipoRepository = relatoriodestinotipoRepository ?? throw new ArgumentNullException(nameof(relatoriodestinotipoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RelatorioDestinoTipoOutputModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _relatoriodestinotipoRepository.GetByAsync(id, cancellationToken);
            return result.IsSuccess ? result.ResultData : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar {EntityName} com ID {Id}", id);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedListNexusResult<RelatorioDestinoTipoOutputModel>> ListPaginadoAsync(PagedFilterInputModel filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _relatoriodestinotipoRepository.GetListAsync(filter, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar {EntityName}s paginados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioDestinoTipoInputModel>> CreateAsync(RelatorioDestinoTipoInputModel relatoriodestinotipo, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioDestinoTipoInputModel>();
        try
        {
            relatoriodestinotipo.Operacao = "I"; // Insert

            retorno = await _relatoriodestinotipoRepository.SalvarAsync(relatoriodestinotipo, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioDestinoTipo criado com sucesso. ID: {Id}", retorno.ResultData.CodDestinoTipo);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar relatoriodestinotipo");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioDestinoTipoInputModel>> UpdateAsync(RelatorioDestinoTipoInputModel relatoriodestinotipo, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioDestinoTipoInputModel>();
        try
        {
            relatoriodestinotipo.Operacao = "U"; // Update

            retorno = await _relatoriodestinotipoRepository.SalvarAsync(relatoriodestinotipo, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioDestinoTipo atualizado com sucesso. ID: {Id}", retorno.ResultData.CodDestinoTipo);
                retorno.AddDefaultSuccessMessage();
                return retorno;
            }

            retorno.AddDefaultFailureMessage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar relatoriodestinotipo");
            retorno.AddFailureMessage(ex.Message);

            return retorno;
        }
        return retorno;
    }

    /// <inheritdoc />
    public async Task<NexusResult<RelatorioDestinoTipoInputModel>> DeleteAsync(RelatorioDestinoTipoInputModel relatoriodestinotipo, CancellationToken cancellationToken = default)
    {
        var retorno = new NexusResult<RelatorioDestinoTipoInputModel>();
        try
        {
            relatoriodestinotipo.Operacao = "D"; // Delete

            retorno = await _relatoriodestinotipoRepository.ExcluirAsync(relatoriodestinotipo, cancellationToken);
            if (retorno.IsSuccess)
            {
                _logger.LogInformation("RelatorioDestinoTipo excluído com sucesso");
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
