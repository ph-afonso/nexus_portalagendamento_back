using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
// Importando todo o stack do Nexus para garantir
using Nexus.Framework.Common;
using Nexus.Framework.Data;
using Nexus.Framework.Data.Model.Result;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Services.Interfaces;

public interface IPortalAgendamentoService
{
    // Consultas
    Task<ClienteOutputModel?> GetCliente(Guid? identificadorCliente, CancellationToken cancellationToken = default);
    Task<PortalAgendamentoOutputModel?> GetDataAgendamentoConfirmacao(Guid? identificadorCliente, CancellationToken cancellationToken = default);
    Task<PortalAgendamentoOutputModel?> GetDataAgendamentoPdf(IFormFile? file, CancellationToken cancellationToken = default);
    Task<PortalAgendamentoOutputModel?> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default);
    Task<PortalAgendamentoOutputModel?> GetValidadeToken(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    // Comandos
    Task<NexusResult<bool>> CreateVoucherTratativa(Guid identificadorCliente, IFormFile file, CancellationToken cancellationToken = default);
    Task<NexusResult<PortalAgendamentoInputModel>> UpdateDataAgendamento(PortalAgendamentoInputModel model, CancellationToken cancellationToken = default);
    Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile file, CancellationToken cancellationToken = default);
}