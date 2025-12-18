using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioModelo
/// </summary>
public class RelatorioModeloOutputModel
{
        [NexusParameter("COD_RELATORIO_MODELO")]
        public int? CodRelatorioModelo { get; set; }

        [NexusParameter("NM_MODELO")]
        public string NmModelo { get; set; }

        [NexusParameter("DS_MODELO")]
        public string DsModelo { get; set; }

        [NexusParameter("NM_PROCEDURE")]
        public string NmProcedure { get; set; }

        [NexusParameter("TP_ARQUIVO_PADRAO")]
        public string TpArquivoPadrao { get; set; }

        [NexusParameter("COD_CATEGORIA")]
        public int? CodCategoria { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("QT_MAX_TENTATIVAS")]
        public int? QtMaxTentativas { get; set; }

        [NexusParameter("QT_TIMEOUT_MINUTOS")]
        public int? QtTimeoutMinutos { get; set; }

        [NexusParameter("COD_USUARIO_CRIACAO")]
        public int? CodUsuarioCriacao { get; set; }

        [NexusParameter("DT_ALTERACAO")]
        public DateTime? DtAlteracao { get; set; }

        [NexusParameter("COD_USUARIO_ALTERACAO")]
        public int? CodUsuarioAlteracao { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
