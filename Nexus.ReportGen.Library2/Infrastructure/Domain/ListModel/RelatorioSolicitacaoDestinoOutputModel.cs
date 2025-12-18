using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioSolicitacaoDestino
/// </summary>
public class RelatorioSolicitacaoDestinoOutputModel
{
        [NexusParameter("COD_SOLICITACAO_DESTINO")]
        public long? CodSolicitacaoDestino { get; set; }

        [NexusParameter("COD_RELATORIO_SOLICITACAO")]
        public long? CodRelatorioSolicitacao { get; set; }

        [NexusParameter("COD_DESTINO_TIPO")]
        public int? CodDestinoTipo { get; set; }

        [NexusParameter("DS_CONFIGURACAO_JSON")]
        public string DsConfiguracaoJson { get; set; }

        [NexusParameter("FL_ANEXAR_ARQUIVO")]
        public bool? FlAnexarArquivo { get; set; }

        [NexusParameter("COD_STATUS_PROCESSAMENTO")]
        public int? CodStatusProcessamento { get; set; }

        [NexusParameter("DT_ENVIO")]
        public DateTime? DtEnvio { get; set; }

        [NexusParameter("MSG_ERRO")]
        public string MsgErro { get; set; }

        [NexusParameter("QT_TENTATIVAS")]
        public int? QtTentativas { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
