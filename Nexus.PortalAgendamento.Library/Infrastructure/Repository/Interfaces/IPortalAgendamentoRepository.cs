using Microsoft.AspNetCore.Http;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Result; // Adicionado para garantir NexusResult
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;
// A linha duplicada foi removida aqui
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;

/// <summary>
/// Interface para o repositório de Bancos
/// </summary>
public interface IPortalAgendamentoRepository
{
    /// <summary>
    /// Consulta Cliente
    /// </summary>
    Task<NexusResult<ClienteOutputModel>> GetCliente(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta data de agendamento
    /// </summary>
    Task<NexusResult<PortalAgendamentoOutputModel>> GetDataAgendamentoConfirmacao(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta Notas do Conhecimento
    /// </summary>
    Task<NexusResult<PortalAgendamentoOutputModel>> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta Validade Token
    /// </summary>
    Task<NexusResult<PortalAgendamentoOutputModel>> GetValidadeToken(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia email com anexo
    /// </summary>
    Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza Data Agendamento
    /// </summary>
    Task<NexusResult<PortalAgendamentoInputModel>> UpdateDataAgendamento(PortalAgendamentoInputModel model, CancellationToken cancellationToken = default);

    Task<NexusResult<bool>> CreateVoucherTratativaAsync(Guid identificadorCliente, IFormFile file, CancellationToken ct = default);
}