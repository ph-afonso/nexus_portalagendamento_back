using Nexus.Framework.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel
{
    public class ClientesRegrasAgendamentoEmail
    {
        [NexusParameter("ID")]
        public string? Id { get; set; }

        [NexusParameter("NOME")]
        public string? Nome { get; set; }

        [NexusParameter("EMAIL")]
        public string? Email { get; set; }

        [NexusParameter("DT_INCLUSAO")]
        public string? DtInclusao { get; set; }

        [NexusParameter("LOGIN_USUARIOS")]
        public string? LoginUsuarios { get; set; }

    }

}
