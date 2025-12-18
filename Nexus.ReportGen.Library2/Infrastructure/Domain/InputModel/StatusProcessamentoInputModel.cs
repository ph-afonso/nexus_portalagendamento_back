using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de StatusProcessamento
/// </summary>
public class StatusProcessamentoInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_STATUS_PROCESSAMENTO", direction: ParameterDirection.InputOutput)]
        public int? CodStatusProcessamento { get; set; }

        [NexusParameter("DS_STATUS")]
        public string DsStatus { get; set; }

        [NexusParameter("SIT_ATIVO")]
        public bool? SitAtivo { get; set; }

        [NexusParameter("COD_USUARIOS")]
        public int? CodUsuarios { get; set; }

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public StatusProcessamentoInputModel()
        {
            _commandName = "APP_TB_STATUS_PROCESSAMENTO";
        }
    }
