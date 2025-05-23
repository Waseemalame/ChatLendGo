using System.Threading.Tasks;

namespace ChatLendGo.Application.Interfaces;

public interface IVoiceInteractionService
{
    Task<string> TranscribeAudioAsync(byte[] audioData);
    Task<byte[]> SynthesizeSpeechAsync(string text);
    Task InitializeStreamingSessionAsync(string conversationId);
    Task<string> ProcessAudioChunkAsync(string sessionId, byte[] audioChunk);
    Task EndStreamingSessionAsync(string sessionId);
}