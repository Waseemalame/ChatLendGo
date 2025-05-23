namespace ChatLendGo.Domain.Interfaces;

public interface IVectorRepository
{
    Task<Guid> StoreVectorAsync(float[] vector, string associatedText, Guid? referenceId = null);
    Task<List<(Guid id, string text, float similarity)>> FindSimilarVectorsAsync(float[] queryVector, int limit = 5, float minSimilarity = 0.7f);
    Task DeleteVectorAsync(Guid id);
}
