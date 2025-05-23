namespace ChatLendGo.Domain.Interfaces;

public interface ILlmProvider
{
    Task<string> GenerateResponseAsync(string prompt, string systemInstruction, List<string> context = null);
    Task<string> GenerateResponseAsync(List<(string content, bool isFromUser)> messages, string systemInstruction);
}
