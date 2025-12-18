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
using Nexus.PortalAgendamento.Library.Infrastructure.Repository.Interfaces;
using Nexus.PortalAgendamento.Library.Infrastructure.Services;

public static class PdfHelper
{
  
    /// <summary>
    /// Converte um arquivo PDF para uma lista de imagens PNG.
    /// </summary>
    /// <param name="caminhoPdf">Caminho para o arquivo PDF.</param>
    /// <param name="dpi">Resolução da imagem em DPI.</param>
    /// <returns>Uma lista de caminhos para as imagens PNG geradas.</returns>
    public static List<string> ConverterPdfParaImagens(string caminhoPdf, int dpi = 300)
    {
        var caminhos = new List<string>();
        var pastaSaida = Path.GetTempPath();

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

                Marshal.Copy(rawBytes, 0, bitmap.GetPixels(), rawBytes.Length);

                var caminhoImagem = Path.Combine(pastaSaida, $"pagina_{i + 1}_{Guid.NewGuid()}.png");
                using var output = File.OpenWrite(caminhoImagem);

                bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(output);
                caminhos.Add(caminhoImagem);

            }
        }
        catch (Exception ex)
        {

            foreach (var caminho in caminhos)
            {
                if (File.Exists(caminho)) File.Delete(caminho);
            }
            caminhos.Clear();
        }

        return caminhos;
    }

    public static string PreprocessarImagemParaOCR(string caminhoImagemOriginal)
    {
        var caminhoPreprocessado = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_preprocessed.png");

        try
        {
            using var src = new Mat(caminhoImagemOriginal, ImreadModes.Grayscale);
            using var preprocessed = new Mat();

            Mat resized = src;
            if (src.Width < 1000 || src.Height < 1000)
            {
                resized = new Mat();
                Cv2.Resize(src, resized, new OpenCvSharp.Size(src.Width * 2, src.Height * 2), 0, 0, InterpolationFlags.Cubic);
            }

            using var denoised = new Mat();
            Cv2.FastNlMeansDenoising(resized, denoised, 10, 7, 21);

            using var contrasted = new Mat();
            Cv2.ConvertScaleAbs(denoised, contrasted, 1.2, 10);

            Cv2.AdaptiveThreshold(contrasted, preprocessed, 255,
                AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 11, 2);

            using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));
            using var cleaned = new Mat();
            Cv2.MorphologyEx(preprocessed, cleaned, MorphTypes.Close, kernel);

            cleaned.SaveImage(caminhoPreprocessado);

            if (resized != src) resized.Dispose();

            return caminhoPreprocessado;
        }
        catch (Exception ex)
        {
            if (File.Exists(caminhoPreprocessado)) File.Delete(caminhoPreprocessado);
            return null;
        }
    }

    public static List<DateTime> ExtrairDatasDoTexto(string texto)
    {
        var datasEncontradas = new List<DateTime>();

        if (string.IsNullOrEmpty(texto))
            return datasEncontradas;

        var padroesDatas = new List<Regex>
    {
        new Regex(@"\b(\d{1,2})[./-](\d{1,2})[./-](\d{4})\b"),
        new Regex(@"\b(\d{4})[./-](\d{1,2})[./-](\d{1,2})\b"),
        new Regex(@"\b(\d{1,2})\s+de\s+(\w+)\s+de\s+(\d{4})\b", RegexOptions.IgnoreCase),
        new Regex(@"\b(\d{1,2})[./-](\d{1,2})[./-](\d{2})\b")
    };

        var mesesPortugues = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        {"janeiro", 1}, {"jan", 1},
        {"fevereiro", 2}, {"fev", 2},
        {"março", 3}, {"mar", 3},
        {"abril", 4}, {"abr", 4},
        {"maio", 5}, {"mai", 5},
        {"junho", 6}, {"jun", 6},
        {"julho", 7}, {"jul", 7},
        {"agosto", 8}, {"ago", 8},
        {"setembro", 9}, {"set", 9},
        {"outubro", 10}, {"out", 10},
        {"novembro", 11}, {"nov", 11},
        {"dezembro", 12}, {"dez", 12}
    };

        foreach (var regex in padroesDatas)
        {
            var matches = regex.Matches(texto);

            foreach (Match match in matches)
            {
                DateTime data = DateTime.MinValue;
                bool dataValida = false;

                try
                {
                    if (regex == padroesDatas[2])
                    {
                        if (int.TryParse(match.Groups[1].Value, out int dia) &&
                            mesesPortugues.TryGetValue(match.Groups[2].Value, out int mes) &&
                            int.TryParse(match.Groups[3].Value, out int ano))
                        {
                            data = new DateTime(ano, mes, dia);
                            dataValida = true;
                        }
                        else continue;
                    }
                    else
                    {
                        var formatos = new string[]
                        {
                        "dd/MM/yyyy", "dd.MM.yyyy", "dd-MM-yyyy",
                        "yyyy/MM/dd", "yyyy.MM.dd", "yyyy-MM-dd",
                        "dd/MM/yy", "dd.MM.yy", "dd-MM-yy",
                        "d/M/yyyy", "d.M.yyyy", "d-M-yyyy"
                        };

                        string valorData = match.Value.Replace('.', '/').Replace('-', '/');

                        foreach (var formato in formatos)
                        {
                            if (DateTime.TryParseExact(valorData, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
                            {
                                if (data.Year < 100)
                                {
                                    data = data.AddYears(2000);
                                }

                                dataValida = true;
                                break;
                            }
                        }
                    }

                    if (dataValida)
                    {
                        if (data.Year >= 1900 && data.Year <= DateTime.Now.Year + 10)
                        {
                            datasEncontradas.Add(data);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        return datasEncontradas.Distinct().OrderBy(d => d).ToList();
    }

    public static PortalAgendamentoOutputModel CriarResultado(List<DateTime> datas)
    {


        return new PortalAgendamentoOutputModel
        {
            DataAgendamentoList = datas
        };
    }

    public static string ExtrairTextoDeImagem(string caminhoImagem)
    {
        var tessdataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");

        try
        {
            using var engine = new TesseractEngine(tessdataPath, "por", EngineMode.Default);

            // Múltiplas configurações para tentar
            var configuracoes = new Dictionary<string, string>
        {
            {"tessedit_pageseg_mode", "6"}, // Assume um único bloco uniforme de texto
            {"tessedit_char_whitelist", "0123456789/-. abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZáéíóúàèìòùâêîôûãõçÁÉÍÓÚÀÈÌÒÙÂÊÎÔÛÃÕÇ"},
            {"tessedit_ocr_engine_mode", "3"}, // Default + LSTM
        };

            foreach (var config in configuracoes)
            {
                engine.SetVariable(config.Key, config.Value);
            }

            using var img = Pix.LoadFromFile(caminhoImagem);
            using var page = engine.Process(img);

            var texto = page.GetText();
            var confianca = page.GetMeanConfidence();


            return texto ?? string.Empty;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
}