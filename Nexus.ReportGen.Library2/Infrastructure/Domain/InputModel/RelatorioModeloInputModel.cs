using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioModelo
/// </summary>
public class RelatorioModeloInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_RELATORIO_MODELO", direction: ParameterDirection.InputOutput)]
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

        /// <summary>
        /// Gets or sets the operation identifier associated with the current context.
        /// </summary>
        [NexusParameter("OPERACAO")]
        [Required]
        public string Operacao { get; set; } = string.Empty;

        public RelatorioModeloInputModel()
        {
            _commandName = "APP_TB_RELATORIO_MODELO";
        }
    }
