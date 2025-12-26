using Docnet.Core;
using Docnet.Core.Models;
using SkiaSharp;
using System.Runtime.InteropServices;
using OpenCvSharp;
using Nexus.PortalAgendamento.Library.Infrastructure.Domain.ListModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Tesseract;
using Microsoft.Extensions.Logging;

namespace Nexus.PortalAgendamento.Library.Infrastructure.Helper;

public class PdfHelper
{
    private readonly ILogger<PdfHelper> _logger;

    public PdfHelper(ILogger<PdfHelper> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Converte um arquivo PDF para uma lista de imagens PNG.
    /// </summary>
    public List<string> ConverterPdfParaImagens(string caminhoPdf, int dpi = 300)
    {
        var caminhos = new List<string>();
        var pastaSaida = Path.GetTempPath();

        if (!File.Exists(caminhoPdf))
        {
            _logger.LogError("ConverterPdfParaImagens -> Arquivo não encontrado: {caminhoPdf}", caminhoPdf);
            return caminhos;
        }

        try
        {
            using var docReader = DocLib.Instance.GetDocReader(caminhoPdf, new PageDimensions(dpi, dpi));
            int totalPaginas = docReader.GetPageCount();

            for (int i = 0; i < totalPaginas; i++)
            {
                using var pageReader = docReader.GetPageReader(i);
                int width = pageReader.GetPageWidth();
                int height = pageReader.GetPageHeight();

                byte[] rawBytes = pageReader.GetImage();
                using var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);

                // Copia os pixels brutos para o Bitmap
                var pixelsPtr = bitmap.GetPixels();
                Marshal.Copy(rawBytes, 0, pixelsPtr, rawBytes.Length);

                var caminhoImagem = Path.Combine(pastaSaida, $"pagina_{i + 1}_{Guid.NewGuid()}.png");

                using var output = File.OpenWrite(caminhoImagem);
                bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(output);

                caminhos.Add(caminhoImagem);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao converter PDF para imagens.");

            // Limpeza em caso de erro
            foreach (var caminho in caminhos)
            {
                if (File.Exists(caminho)) File.Delete(caminho);
            }
            caminhos.Clear();
            throw; // Re-lança para quem chamou saber que falhou
        }

        return caminhos;
    }

    public string? PreprocessarImagemParaOCR(string caminhoImagemOriginal)
    {
        var caminhoPreprocessado = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_preprocessed.png");

        try
        {
            using var src = new Mat(caminhoImagemOriginal, ImreadModes.Grayscale);

            // Redimensionamento se a imagem for muito pequena
            Mat resized = src;
            bool needsDisposeResized = false;

            if (src.Width < 1000 || src.Height < 1000)
            {
                resized = new Mat();
                Cv2.Resize(src, resized, new OpenCvSharp.Size(src.Width * 2, src.Height * 2), 0, 0, InterpolationFlags.Cubic);
                needsDisposeResized = true;
            }

            using var denoised = new Mat();
            Cv2.FastNlMeansDenoising(resized, denoised, 10, 7, 21);

            using var contrasted = new Mat();
            Cv2.ConvertScaleAbs(denoised, contrasted, 1.2, 10); // Aumento de contraste

            using var preprocessed = new Mat();
            Cv2.AdaptiveThreshold(contrasted, preprocessed, 255,
                AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 11, 2);

            // Reduz ruído (pontos isolados)
            using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));
            using var cleaned = new Mat();
            Cv2.MorphologyEx(preprocessed, cleaned, MorphTypes.Close, kernel);

            cleaned.SaveImage(caminhoPreprocessado);

            if (needsDisposeResized) resized.Dispose();

            return caminhoPreprocessado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no pré-processamento da imagem: {caminho}", caminhoImagemOriginal);
            if (File.Exists(caminhoPreprocessado)) File.Delete(caminhoPreprocessado);
            return null;
        }
    }

    public List<DateTime> ExtrairDatasDoTexto(string texto)
    {
        var datasEncontradas = new List<DateTime>();

        if (string.IsNullOrWhiteSpace(texto))
            return datasEncontradas;

        // Regex melhorado para evitar falsos positivos
        var padroesDatas = new List<Regex>
        {
            new Regex(@"\b(\d{1,2})[./-](\d{1,2})[./-](\d{4})\b"), // dd/MM/yyyy
            new Regex(@"\b(\d{4})[./-](\d{1,2})[./-](\d{1,2})\b"), // yyyy-MM-dd
            new Regex(@"\b(\d{1,2})\s+de\s+(\w+)\s+de\s+(\d{4})\b", RegexOptions.IgnoreCase), // 10 de Janeiro de 2023
            new Regex(@"\b(\d{1,2})[./-](\d{1,2})[./-](\d{2})\b")  // dd/MM/yy
        };

        var mesesPortugues = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"janeiro", 1}, {"jan", 1}, {"fevereiro", 2}, {"fev", 2},
            {"março", 3}, {"mar", 3}, {"abril", 4}, {"abr", 4},
            {"maio", 5}, {"mai", 5}, {"junho", 6}, {"jun", 6},
            {"julho", 7}, {"jul", 7}, {"agosto", 8}, {"ago", 8},
            {"setembro", 9}, {"set", 9}, {"outubro", 10}, {"out", 10},
            {"novembro", 11}, {"nov", 11}, {"dezembro", 12}, {"dez", 12}
        };

        foreach (var regex in padroesDatas)
        {
            var matches = regex.Matches(texto);

            foreach (Match match in matches)
            {
                try
                {
                    DateTime data = DateTime.MinValue;
                    bool dataValida = false;

                    if (regex == padroesDatas[2]) // Formato Extenso
                    {
                        if (int.TryParse(match.Groups[1].Value, out int dia) &&
                            mesesPortugues.TryGetValue(match.Groups[2].Value, out int mes) &&
                            int.TryParse(match.Groups[3].Value, out int ano))
                        {
                            if (ano < 100) ano += 2000; // Ajuste ano curto se necessário
                            try { data = new DateTime(ano, mes, dia); dataValida = true; } catch { }
                        }
                    }
                    else // Formatos numéricos
                    {
                        var formatos = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "dd/MM/yy", "d/M/yyyy" };
                        var valorData = match.Value.Replace('.', '/').Replace('-', '/');

                        if (DateTime.TryParse(valorData, new CultureInfo("pt-BR"), DateTimeStyles.None, out data))
                        {
                            dataValida = true;
                        }
                    }

                    if (dataValida)
                    {
                        // Validação de intervalo lógico (ex: não pegar datas de 1900 ou 2099)
                        if (data.Year >= 2020 && data.Year <= DateTime.Now.Year + 5)
                        {
                            datasEncontradas.Add(data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log silencioso para não parar o loop
                    _logger.LogWarning(ex, "Falha ao parsear data potencial: {valor}", match.Value);
                }
            }
        }

        return datasEncontradas.Distinct().OrderBy(d => d).ToList();
    }

    public string ExtrairTextoDeImagem(string caminhoImagem)
    {
        var tessdataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");

        if (!File.Exists(caminhoImagem)) return string.Empty;

        try
        {
            using var engine = new TesseractEngine(tessdataPath, "por", EngineMode.Default);

            // Configurações otimizadas
            engine.SetVariable("tessedit_char_whitelist", "0123456789/-. abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZáéíóúàèìòùâêîôûãõçÁÉÍÓÚÀÈÌÒÙÂÊÎÔÛÃÕÇ");
            engine.SetVariable("tessedit_pageseg_mode", "3"); // 3 = Auto (Detecta blocos, ao invés de 6 que assume bloco único)

            using var img = Pix.LoadFromFile(caminhoImagem);
            using var page = engine.Process(img);

            var texto = page.GetText();
            var confianca = page.GetMeanConfidence();

            _logger.LogDebug("OCR Concluído. Confiança: {conf}. Texto: {len} chars", confianca, texto?.Length ?? 0);

            return texto ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro fatal no Tesseract OCR para imagem: {caminho}", caminhoImagem);
            return string.Empty;
        }
    }

    public static PortalAgendamentoOutputModel CriarResultado(List<DateTime> datas)
    {
        return new PortalAgendamentoOutputModel
        {
            DataAgendamentoList = datas,
            // Preenche o campo formatado com a primeira data encontrada, se houver
            DataAgendamentoFormatted = datas.FirstOrDefault().ToString("dd/MM/yyyy")
        };
    }
}