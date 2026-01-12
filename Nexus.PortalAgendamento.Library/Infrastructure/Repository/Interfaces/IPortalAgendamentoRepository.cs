using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;

public interface IPortalAgendamentoRepository
{
    Task<NexusResult<ValidadeTokenOutputModel>> ValidarTokenAsync(ValidadeTokenInputModel model, CancellationToken cancellationToken = default);
    Task<NexusResult<PortalAgendamentoOutputModel>> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default);
    Task<OcorrenciaAbertaModel?> GetOcorrenciaAbertaAsync(long numeroNota, int codFilial, CancellationToken cancellationToken = default);
    Task EncerrarOcorrenciaMassaAsync(OcorrenciaAbertaModel ocorrencia, CancellationToken cancellationToken = default);
    Task<NexusResult<bool>> RealizarAgendamentoAsync(AgendamentoRepositoryInputModel model, CancellationToken cancellationToken = default);
    Task<List<ClientesRegrasAgendamentoEmail>> GetDestinatariosEmailAsync(int? codFornecedor, int? codRecebedor, CancellationToken cancellationToken = default);
    Task EnviarEmailPostfixAsync(EmailPostFixInputModel input, CancellationToken cancellationToken = default);
    Task<NexusResult<bool>> VincularAnexoOcorrenciaAsync(int codOcorrencia, string nomeArquivoOriginal, byte[] arquivoBytes, CancellationToken ct);
}