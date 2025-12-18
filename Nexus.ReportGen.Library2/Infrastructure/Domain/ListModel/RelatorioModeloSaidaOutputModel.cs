using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioModeloSaida
/// </summary>
public class RelatorioModeloSaidaOutputModel
{
        [NexusParameter("COD_MODELO_SAIDA")]
        public int? CodModeloSaida { get; set; }

        [NexusParameter("COD_RELATORIO_MODELO")]
        public int? CodRelatorioModelo { get; set; }

        [NexusParameter("NM_ABA_ARQUIVO")]
        public string NmAbaArquivo { get; set; }

        [NexusParameter("DS_SAIDA")]
        public string DsSaida { get; set; }

        [NexusParameter("NR_ORDEM")]
        public int? NrOrdem { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
