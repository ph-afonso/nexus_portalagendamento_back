using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioCategoria
/// </summary>
public class RelatorioCategoriaOutputModel
{
        [NexusParameter("COD_CATEGORIA")]
        public int? CodCategoria { get; set; }

        [NexusParameter("NM_CATEGORIA")]
        public string NmCategoria { get; set; }

        [NexusParameter("DS_CATEGORIA")]
        public string DsCategoria { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("NR_ORDEM")]
        public int? NrOrdem { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
