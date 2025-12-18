using Nexus.Framework.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.ListModel
{
    public class NotaFiscalOutputModel
    {
        [NexusParameter("NR_PEDIDO")]
        public string? NrPedido { get; set; }

        [NexusParameter("NOME_RECEBEDOR")]
        public string? NomeRecebedor { get; set; }

        [NexusParameter("NOME_FORNECEDOR")]
        public string? NomeFornecedor { get; set; }

        [NexusParameter("UF_CLIENTES")]
        public string? UFClientes { get; set; }

        [NexusParameter("NR_NOTAS_FISCAIS")]
        public int? NrNotasFiscais { get; set; }

        [NexusParameter("NR_SERIE_NOTAS_FISCAIS")]
        public string? NrSerieNotasFiscais { get; set; }

        [NexusParameter("DT_EMISSAO")]
        public string? DtEmissao { get; set; }

        [NexusParameter("QT_VOLUME_NOTAS_FISCAIS")]
        public int? QtVolumeNotasFiscais { get; set; }

        [NexusParameter("PESO_NOTAS_FISCAIS")]
        public decimal? PesoNotasFiscais { get; set; }

        [NexusParameter("VL_TOTAL_NOTAS_FISCAIS")]
        public decimal? VlTotalNotasFiscais { get; set; }

        [NexusParameter("COD_CLIENTES_FORNECEDOR")]
        public int? CodClientesFornecedor { get; set; }

        [NexusParameter("COD_CLIENTES_RECEBEDOR")]
        public int? CodClientesRecebedor { get; set; }


    }

}
