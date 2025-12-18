using Nexus.Framework.Data.Attributes;

namespace Nexus.Sample.Library.Infrastructure.Domain.ListModel;

/// <summary>
/// Modelo de saída para consultas de Bancos
/// </summary>
public class BancosOutputModel
{
        [NexusParameter("COD_BANCOS")]
        public int? CodBancos { get; set; }

        [NexusParameter("DS_BANCOS")]
        public string DsBancos { get; set; }

        [NexusParameter("ISPB_BANCOS")]
        public string IspbBancos { get; set; }

        [NexusParameter("FL_EMITE_BOLETO_BANCOS")]
        public bool? FlEmiteBoletoBancos { get; set; }

        [NexusParameter("COD_DIGITO_BANCOS")]
        public string CodDigitoBancos { get; set; }

        [NexusParameter("FL_EXCLUIDO")]
        public bool? FlExcluido { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

}
