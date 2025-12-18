using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioModeloDestino
/// </summary>
public class RelatorioModeloDestinoOutputModel
{
        [NexusParameter("COD_MODELO_DESTINO")]
        public int? CodModeloDestino { get; set; }

        [NexusParameter("COD_RELATORIO_MODELO")]
        public int? CodRelatorioModelo { get; set; }

        [NexusParameter("COD_DESTINO_TIPO")]
        public int? CodDestinoTipo { get; set; }

        [NexusParameter("FL_NOTIFICAR_SOLICITANTE")]
        public bool? FlNotificarSolicitante { get; set; }

        [NexusParameter("DS_CONFIGURACAO_JSON")]
        public string DsConfiguracaoJson { get; set; }

        [NexusParameter("FL_ANEXAR_ARQUIVO")]
        public bool? FlAnexarArquivo { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
