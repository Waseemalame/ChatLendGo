using ChatLendGo.Domain.Enums;

namespace ChatLendGo.Domain.Interfaces;

public interface IEmbeddingProvider
{
    Task<float[]> GenerateEmbeddingAsync(string text);
    EmbeddingType EmbeddingType { get; }
}
