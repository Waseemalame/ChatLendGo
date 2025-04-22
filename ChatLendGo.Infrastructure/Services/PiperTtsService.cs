using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ChatLendGo.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace ChatLendGo.Infrastructure.Services;

public class PiperTtsService
{
    private readonly PiperSettings _settings;

    public PiperTtsService(IOptions<PiperSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<byte[]> SynthesizeSpeechAsync(string text)
    {
        var tempTextFile = Path.GetTempFileName();
        var tempWavFile = Path.GetTempFileName() + ".wav";

        try
        {
            await File.WriteAllTextAsync(tempTextFile, text);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _settings.PiperExecutablePath,
                    Arguments = $"--model {_settings.ModelPath} --output_file {tempWavFile} {tempTextFile}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"Piper TTS error: {error}");
            }

            return await File.ReadAllBytesAsync(tempWavFile);
        }
        finally
        {
            if (File.Exists(tempTextFile))
                File.Delete(tempTextFile);

            if (File.Exists(tempWavFile))
                File.Delete(tempWavFile);
        }
    }
}

public class PiperSettings
{
    public string PiperExecutablePath { get; set; } = "piper";
    public string ModelPath { get; set; } = "en_US-lessac-medium.onnx";
}