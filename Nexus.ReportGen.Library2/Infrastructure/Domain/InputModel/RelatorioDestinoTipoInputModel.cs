using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioDestinoTipo
/// </summary>
public class RelatorioDestinoTipoInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_DESTINO_TIPO", direction: ParameterDirection.InputOutput)]
        public int? CodDestinoTipo { get; set; }

        [NexusParameter("NM_TIPO")]
        public string NmTipo { get; set; }

        [NexusParameter("DS_TIPO")]
        public string DsTipo { get; set; }

        [NexusParameter("FL_ATIVO")]
        public bool? FlAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public RelatorioDestinoTipoInputModel()
        {
            _commandName = "APP_TB_RELATORIO_DESTINO_TIPO";
        }
    }
