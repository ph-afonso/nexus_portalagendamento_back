using Microsoft.AspNetCore.Http;
using Nexus.Framework.Common;
using Nexus.Framework.Data.Model.Input;
using Nexus.Framework.Data.Model.Result;
using Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;
using Nexus.Sample.Library.Infrastructure.Domain;
using Nexus.Sample.Library.Infrastructure.Domain.InputModel;
using Nexus.Sample.Library.Infrastructure.Domain.ListModel;

namespace Nexus.Sample.Library.Infrastructure.Services.Interfaces;

/// <summary>
/// Interface para o serviço de gerenciamento de Bancos
/// </summary>
public interface IPortalAgendamentoService
{
    /// <summary>
    /// Consulta Cliente
    /// </summary>
    /// <param name="identificadorCliente">Identificador do Portal Agendamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Data de agendamento</returns>
    Task<ClienteOutputModel> GetCliente(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta data de agendamento
    /// </summary>
    /// <param name="identificadorCliente">Identificador do Portal Agendamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Data de agendamento</returns>
    Task<PortalAgendamentoOutputModel> GetDataAgendamentoConfirmacao(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extrai data de agendamento do PDF
    /// </summary>
    /// <param name="file">Arquivo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Data de agendamento</returns>
    Task<PortalAgendamentoOutputModel> GetDataAgendamentoPdf(IFormFile? file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém Notas do conhecimento
    /// </summary>
    /// <param name="identificadorCliente">Identificador do Portal Agendamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>O notas encontrado ou null</returns>
    Task<PortalAgendamentoOutputModel> GetNotasConhecimento(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtem validade do Token
    /// </summary>
    /// <param name="identificadorCliente">Identificador do Portal Agendamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Data da validade</returns>
    Task<PortalAgendamentoOutputModel> GetValidadeToken(Guid? identificadorCliente, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia email com anexo
    /// </summary>
    /// <param name="request">Arquivo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    Task<NexusResult<EmailPostFixInputModel>> SendEmailAnexo(Guid? identificadorCliente, IFormFile request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza Data Agendamento
    /// </summary>
    /// <param name="model">Dados do protal agendamento a ser atualizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com o bancos criado</returns>
    Task<NexusResult<PortalAgendamentoInputModel>> UpdateDataAgendamento(PortalAgendamentoInputModel model, CancellationToken cancellationToken = default);

    Task<NexusResult<bool>> CreateVoucherTratativaAsync(Guid identificadorCliente, IFormFile file, CancellationToken ct = default);




}
