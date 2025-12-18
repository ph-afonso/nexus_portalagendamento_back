using Nexus.Framework.Data.Attributes;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de RelatorioModeloParametro
/// </summary>
public class RelatorioModeloParametroOutputModel
{
        [NexusParameter("COD_PARAMETRO")]
        public int? CodParametro { get; set; }

        [NexusParameter("COD_RELATORIO_MODELO")]
        public int? CodRelatorioModelo { get; set; }

        [NexusParameter("NM_PARAMETRO")]
        public string NmParametro { get; set; }

        [NexusParameter("DS_PARAMETRO")]
        public string DsParametro { get; set; }

        [NexusParameter("COD_TIPO_PARAMETRO")]
        public int? CodTipoParametro { get; set; }

        [NexusParameter("NR_ORDEM")]
        public int? NrOrdem { get; set; }

        [NexusParameter("FL_OBRIGATORIO")]
        public bool? FlObrigatorio { get; set; }

        [NexusParameter("VL_PADRAO")]
        public string VlPadrao { get; set; }

        [NexusParameter("DS_HELP")]
        public string DsHelp { get; set; }

        [NexusParameter("QT_MIN_CARACTERES")]
        public int? QtMinCaracteres { get; set; }

        [NexusParameter("QT_MAX_CARACTERES")]
        public int? QtMaxCaracteres { get; set; }

        [NexusParameter("VL_MINIMO")]
        public decimal? VlMinimo { get; set; }

        [NexusParameter("VL_MAXIMO")]
        public decimal? VlMaximo { get; set; }

        [NexusParameter("FL_VISIVEL")]
        public bool? FlVisivel { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
