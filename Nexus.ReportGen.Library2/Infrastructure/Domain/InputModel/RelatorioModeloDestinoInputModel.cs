using System.ComponentModel.DataAnnotations;
using System.Data;
using Nexus.Framework.Data.Attributes;
using Nexus.Framework.Data.Model.Input;

namespace Nexus.ReportGen.Library.Infrastructure.Domain.InputModel;

/// <summary>
/// Modelo de entrada para operações de RelatorioModeloDestino
/// </summary>
public class RelatorioModeloDestinoInputModel : StoredProcedureInputModel
{
        [NexusParameter("COD_MODELO_DESTINO", direction: ParameterDirection.InputOutput)]
        public int? CodModeloDestino { get; set; }

        [NexusParameter("COD_RELATORIO_MODELO")]
        public int? CodRelatorioModelo { get; set; }

        [NexusParameter("COD_DESTINO_TIPO")]
        public int? CodDestinoTipo { get; set; }

        [NexusParameter("FL_NOTIFICAR_SOLICITANTE")]
        public bool? FlNotificarSolicitante { get; set; }

        [NexusParameter("DS_CONFIGURACAO_JSON")]
        public string DsConfiguracaoJson { get; set; }

        [NexusParameter("FL_ANEXAR_ARQUIVO")]
        public bool? FlAnexarArquivo { get; set; }

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

        public RelatorioModeloDestinoInputModel()
        {
            _commandName = "APP_TB_RELATORIO_MODELO_DESTINO";
        }
    }
