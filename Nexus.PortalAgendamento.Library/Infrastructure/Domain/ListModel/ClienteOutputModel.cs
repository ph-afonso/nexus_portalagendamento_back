using Nexus.Framework.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel
{
    public class ClienteOutputModel
    {
        [NexusParameter("NM_RAZAO_CLIENTES")]
        public string? NomeCliente { get; set; }

    }

}
