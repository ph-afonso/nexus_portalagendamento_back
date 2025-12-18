using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de Bancos
/// </summary>
public class EmailPostFixInputModel : StoredProcedureInputModel
{

    [NexusParameter("COD_REMETENTE_PESSOA")]
    public int? CodRemetentePessoa { get; set; }


    [NexusParameter("COD_EVENTOS")]
    public short? CodEventos { get; set; }


    [NexusParameter("DS_ASSUNTO")]
    public string? DsAssunto { get; set; }


    [NexusParameter("DS_MENSAGEM")]
    public string? DsMensagem { get; set; }


    [NexusParameter("FL_MENSAGEM_HTML")]
    public bool? FlMensagemHtml { get; set; }


    [NexusParameter("LOGIN_REMETENTE_USUARIOS")]
    public string? LoginRemetenteUsuarios { get; set; }


    [NexusParameter("DESTINATARIOS_JSON")]
    public string? DestinatariosJson { get; set; }


    [NexusParameter("NM_ARQUIVO")]
    public string? NmArquivo { get; set; }


    [NexusParameter("MIME_TYPE")]
    public string? MimeType { get; set; }


    [NexusParameter("ARQUIVO")]
    public byte[]? Arquivo { get; set; }


    [NexusParameter("EMAIL_REMETENTE")]
    public string? EmailRemetente { get; set; }


    [NexusParameter("CAMINHO_ARQUIVO")]
    public string? CaminhoArquivo { get; set; }


    public EmailPostFixInputModel()
    {
        _commandName = "NewSitex.dbo.PR_ENVIAR_EMAIL_POSTFIX";
    }
}
