using Microsoft.AspNetCore.Http;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;

public interface IPortalAgendamentoRepository
{
    // --- Métodos Existentes (Legado/Padrão) ---
    Task<NexusResult<ClienteOutputModel>> GetCliente(Guid? identificadorCliente, CancellationToken cancellationToken = default);
    Task<NexusResult<PortalAgendamentoOutputModel>> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    // Método antigo de validade (Se ainda estiver sendo usado por outros endpoints antigos, mantenha. Se não, pode remover)
    Task<NexusResult<PortalAgendamentoOutputModel>> GetValidadeToken(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    //Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile request, CancellationToken cancellationToken = default);
    Task<NexusResult<PortalAgendamentoInputModel>> UpdateDataAgendamento(PortalAgendamentoInputModel model, CancellationToken cancellationToken = default);
    Task<NexusResult<bool>> CreateVoucherTratativaAsync(Guid identificadorCliente, IFormFile file, CancellationToken ct = default);

    // --- NOVOS MÉTODOS (Refatorados/Adicionados) ---

    /// <summary>
    /// Busca as datas de inclusão/alteração para cálculo de validade (Usa InputModel)
    /// </summary>
    Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken cancellationToken = default);
    /// <summary>
    /// Realiza o agendamento das notas fiscais
    /// </summary>
    Task<NexusResult<bool>> RealizarAgendamentoAsync(AgendamentoRepositoryInputModel input, CancellationToken cancellationToken = default);
    Task<IEnumerable<OcorrenciaImpeditivaModel>> CheckOcorrenciasImpeditivasAsync(long nrNota, int codFilial, CancellationToken cancellationToken = default);
    Task EncerrarOcorrenciaMassaAsync(OcorrenciaAbertaModel ocorrencia, CancellationToken cancellationToken = default);
    Task<OcorrenciaAbertaModel?> GetOcorrenciaAbertaAsync(long nrNota, int codFilial, CancellationToken cancellationToken = default);
    Task EnviarEmailPostfixAsync(EmailPostFixInputModel input, CancellationToken cancellationToken = default);
    Task<List<ClientesRegrasAgendamentoEmail>> GetDestinatariosEmailAsync(int? codFornecedor, int? codRecebedor, CancellationToken cancellationToken = default);
}