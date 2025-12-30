using System.Globalization;
using System.Text;
using System.Net;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Common;

public static class EmailTemplateBuilder
{
    private static readonly CultureInfo _culture = new CultureInfo("pt-BR");

    /// <summary>
    /// Gera HTML para o caso de falha na leitura do anexo (Lista de NFs impactadas).
    /// </summary>
    public static string BuildMensagemComTabelaNFs(IEnumerable<NotaFiscalOutputModel> notas)
    {
        var sb = new StringBuilder();

        sb.Append("""
        <div style="font-family: Arial, Helvetica, sans-serif; font-size:14px; line-height:1.5; color:#222">
          <p style="margin:0 0 12px 0">
            O cliente tentou agendar as NF's abaixo, porém o anexo enviado não pode ser lido corretamente
            pelo sistema. Sendo assim, é necessário entrar em contato com o cliente e seguir com o processo
            de forma manual.
          </p>

          <h4 style="margin:18px 0 8px 0; font-size:16px; font-weight:bold; color:#0056b3;">Notas Fiscais</h4>

          <table role="table" cellpadding="0" cellspacing="0" border="0"
                 style="border-collapse:collapse; width:100%; max-width:900px;">
            <thead>
              <tr>
                <th style="text-align:left; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Pedido</th>
                <th style="text-align:left; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Fornecedor</th>
                <th style="text-align:left; padding:8px; border:1px solid #ddd; background:#f5f5f5;">UF</th>
                <th style="text-align:right; padding:8px; border:1px solid #ddd; background:#f5f5f5;">NF</th>
                <th style="text-align:left; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Série</th>
                <th style="text-align:center; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Emissão</th>
                <th style="text-align:right; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Volumes</th>
                <th style="text-align:right; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Peso</th>
                <th style="text-align:right; padding:8px; border:1px solid #ddd; background:#f5f5f5;">Valor Total</th>
              </tr>
            </thead>
            <tbody>
        """);

        if (notas != null)
        {
            foreach (var n in notas)
            {
                var pedido = HtmlEncode(n.NrPedido);
                var fornecedor = HtmlEncode(n.NomeFornecedor);
                var uf = HtmlEncode(n.UFClientes);
                var nf = n.NrNotasFiscais?.ToString(_culture) ?? "";
                var serie = HtmlEncode(n.NrSerieNotasFiscais);
                var emissao = HtmlEncode(n.DtEmissao);
                var volumes = n.QtVolumeNotasFiscais?.ToString(_culture) ?? "";
                var peso = n.PesoNotasFiscais.HasValue ? n.PesoNotasFiscais.Value.ToString("#,0.###", _culture) : "";
                var valor = n.VlTotalNotasFiscais.HasValue ? n.VlTotalNotasFiscais.Value.ToString("C", _culture) : "";

                sb.Append($@"
                  <tr>
                    <td style=""padding:8px; border:1px solid #ddd;"">{pedido}</td>
                    <td style=""padding:8px; border:1px solid #ddd;"">{fornecedor}</td>
                    <td style=""padding:8px; border:1px solid #ddd;"">{uf}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:right;"">{nf}</td>
                    <td style=""padding:8px; border:1px solid #ddd;"">{serie}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:center;"">{emissao}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:right;"">{volumes}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:right;"">{peso}</td>
                    <td style=""padding:8px; border:1px solid #ddd; text-align:right;"">{valor}</td>
                  </tr>");
            }
        }

        sb.Append("""
            </tbody>
          </table>
        </div>
        """);

        return sb.ToString();
    }

    /// <summary>
    /// Gera HTML para o Resumo do Agendamento Automático (Sucessos e Bloqueios).
    /// </summary>
    public static string BuildResumoProcessamento(List<AgendamentoDetalheModel> resultados, string nomeSolicitante)
    {
        var sb = new StringBuilder();

        int total = resultados.Count;
        int sucesso = resultados.Count(x => x.Agendado);
        int falha = total - sucesso;

        // 1. CABEÇALHO HTML (ESTILO TRAGETTA)
        sb.Append("""
        <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
        <html xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office">
        <head>
            <meta http-equiv="Content-type" content="text/html; charset=utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
            <meta http-equiv="X-UA-Compatible" content="IE=edge" />
            <link href="https://fonts.googleapis.com/css?family=Noto+Sans:400,400i,700,700i" rel="stylesheet" />
            <title>Resumo de Agendamento - Tragetta</title>
            <style type="text/css" media="screen">
                body { padding: 0 !important; margin: 0 !important; display: block !important; min-width: 100% !important; width: 100% !important; background: #f4f4f4; -webkit-text-size-adjust: none; font-family: 'Noto Sans', Arial, sans-serif; }
                a { color: #00b200; text-decoration: none; }
                p { padding: 0 !important; margin: 0 !important; }
                img { -ms-interpolation-mode: bicubic; }
                .mcnPreviewText { display: none !important; }
                /* Mobile */
                @media only screen and (max-device-width: 640px), only screen and (max-width: 640px) {
                    .mobile-shell { width: 100% !important; min-width: 100% !important; }
                    .text-header, .m-center { text-align: center !important; }
                    .center { margin: 0 auto !important; }
                    .container { padding: 20px 10px !important; }
                    .td { width: 100% !important; min-width: 100% !important; }
                    .p30-15 { padding: 30px 15px !important; }
                    .p20 { padding: 20px !important; }
                    .table-scroll-wrapper { overflow-x: auto !important; display: block !important; width: 100% !important; -webkit-overflow-scrolling: touch; }
                }
            </style>
        </head>
        <body class="body" style="padding:0 !important; margin:0 !important; display:block !important; min-width:100% !important; width:100% !important; background:#f4f4f4; -webkit-text-size-adjust:none;">
            <table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#f4f4f4">
                <tr>
                    <td align="center" valign="top">
                        <table width="650" border="0" cellspacing="0" cellpadding="0" class="mobile-shell" bgcolor="#ffffff" style="margin: 50px 0;">
                            
                            <tr>
                                <td class="td container" style="width:650px; min-width:650px; font-size:0pt; line-height:0pt; margin:0; font-weight:normal; padding:30px 40px;">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td align="left">
                                                <a href="https://www.tragetta.com.br" target="_blank">
                                                    <img src="https://newsitex.expressojundiai.com.br/Femsa.Zeus/Repository/Comunicado/logo-tragetta.png" width="167" border="0" alt="Tragetta" title="Tragetta" />
                                                </a>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td bgcolor="#0B0B0B" style="padding: 25px 0;">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="text-center" style="color:#00b200; font-family:'Noto Sans', Arial,sans-serif; font-size:26px; line-height:30px; text-align:center; font-weight: bold;">
                                                Resumo de Processamento
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td class="p30-15" style="padding: 40px 40px 10px 40px;">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        
                                        <tr>
                                            <td class="text pb20" style="color:#424243; font-family:'Noto Sans', Arial,sans-serif; font-size:16px; line-height:26px; text-align:left; padding-bottom: 20px;">
        """);

        // INJETANDO DADOS DINÂMICOS
        sb.Append($"O processamento automático de agendamento foi finalizado em <b>{DateTime.Now:dd/MM/yyyy HH:mm}</b>.<br/><br/>");

        // BOX DE RESUMO NUMÉRICO
        sb.Append("<div style='background-color:#f9f9f9; padding:15px; border-left:4px solid #00b200;'>");
        sb.Append($"<b>Total de Notas:</b> {total} &nbsp;|&nbsp; ");
        sb.Append($"<span style='color:#28a745'><b>Sucesso:</b> {sucesso}</span> &nbsp;|&nbsp; ");
        sb.Append($"<span style='color:#dc3545'><b>Bloqueadas:</b> {falha}</span>");
        sb.Append("</div>");

        sb.Append("""
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="table-scroll-wrapper" style="padding-bottom: 30px; padding-top: 20px;">
                                                <table width="100%" border="0" cellpadding="5" cellspacing="0" style="border-collapse: collapse; border: 1px solid #eee;">
                                                    <thead>
                                                        <tr style="background-color: #f8f9fa; border-bottom: 2px solid #ddd;">
                                                            <th style="text-align:left; font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243; font-weight:bold;">Pedido</th>
                                                            <th style="text-align:left; font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243; font-weight:bold;">NF - Série</th>
                                                            <th style="text-align:left; font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243; font-weight:bold;">Fornecedor</th>
                                                            <th style="text-align:left; font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243; font-weight:bold;">Recebedor</th>
                                                            <th style="text-align:center; font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243; font-weight:bold;">Status</th>
                                                            <th style="text-align:center; font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243; font-weight:bold;">Data</th>
                                                            <th style="text-align:left; font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243; font-weight:bold;">Observação</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
        """);

        // LOOP DAS LINHAS DA TABELA
        foreach (var item in resultados)
        {
            string corTexto = item.Agendado ? "#28a745" : "#dc3545"; // Verde ou Vermelho
            string bgRow = item.Agendado ? "#ffffff" : "#fff5f5";

            // Azul para "Já Agendado"
            if (item.Status != null && item.Status.Contains("Já Agendado", StringComparison.OrdinalIgnoreCase))
            {
                corTexto = "#17a2b8"; // Azul Tragetta
                bgRow = "#f0fbff";
            }

            var dataHora = (item.Agendado || (item.Status != null && item.Status.Contains("Já Agendado", StringComparison.OrdinalIgnoreCase)))
                ? $"{item.DataAgendada} {item.HoraAgendada}"
                : "-";

            // Formatação NF-Série
            var nfSerie = $"{item.NrNota}-{item.Serie}";

            // Proteção HTML
            var pedidoSafe = HtmlEncode(item.Pedido);
            var nfSafe = HtmlEncode(nfSerie);
            var fornSafe = HtmlEncode(item.NomeFornecedor);
            var recSafe = HtmlEncode(item.NomeRecebedor);
            var statusSafe = HtmlEncode(item.Status);
            var obsSafe = HtmlEncode(item.Mensagem);

            sb.Append($@"
                <tr style=""background-color: {bgRow}; border-bottom: 1px solid #eee;"">
                    <td style=""font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243;"">{pedidoSafe}</td>
                    <td style=""font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243;""><b>{nfSafe}</b></td>
                    <td style=""font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243;"">{fornSafe}</td>
                    <td style=""font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#424243;"">{recSafe}</td>
                    <td style=""font-family:'Noto Sans', Arial,sans-serif; font-size:11px; text-align:center; font-weight:bold; color:{corTexto};"">{statusSafe}</td>
                    <td style=""font-family:'Noto Sans', Arial,sans-serif; font-size:11px; text-align:center; color:#424243;"">{dataHora}</td>
                    <td style=""font-family:'Noto Sans', Arial,sans-serif; font-size:11px; color:#6d6e70;"">{obsSafe}</td>
                </tr>");
        }

        // RODAPÉ
        sb.Append("""
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td bgcolor="#f8f9fa" style="padding: 30px 40px; border-top: 30px solid #000;">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="text-footer1" style="color:#6d6e70; font-family:'Noto Sans', Arial,sans-serif; font-size:12px; line-height:20px; padding-bottom:5px;">
                                                Atenciosamente,
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="text-footer2" style="color:#424243; font-family:'Noto Sans', Arial,sans-serif; font-size:14px; line-height:20px; font-weight: bold; padding-bottom:15px;">
                                                Equipe de Agendamento Tragetta<br />
                                                Portal Agendamento DPA
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="padding-top: 10px;">
                                                <table border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td class="img" width="38" style="font-size:0pt; line-height:0pt; text-align:left;">
                                                            <a href="https://www.instagram.com/tragettaoficial" target="_blank">
                                                                <img src="https://www.8tracker.com.br/img/icon-instagram.jpg" width="38" height="38" border="0" alt="Instagram" />
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>
        """);

        return sb.ToString();
    }

    private static string HtmlEncode(string? s)
    {
        return string.IsNullOrEmpty(s) ? "" : WebUtility.HtmlEncode(s);
    }
}