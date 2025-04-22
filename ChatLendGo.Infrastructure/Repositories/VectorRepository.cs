using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatLendGo.Domain.Interfaces;
using ChatLendGo.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatLendGo.Infrastructure.Repositories;

public class VectorRepository : IVectorRepository
{
    private readonly IMongoCollection<VectorEntry> _vectors;

    public VectorRepository(MongoDbContext context)
    {
        _vectors = context.GetCollection<VectorEntry>("vector_embeddings");
    }

    public async Task<Guid> StoreVectorAsync(float[] vector, string associatedText, Guid? referenceId = null)
    {
        var entry = new VectorEntry
        {
            Id = Guid.NewGuid(),
            Vector = vector,
            Text = associatedText,
            ReferenceId = referenceId,
            CreatedAt = DateTime.UtcNow
        };

        await _vectors.InsertOneAsync(entry);
        return entry.Id;
    }

    public async Task<List<(Guid id, string text, float similarity)>> FindSimilarVectorsAsync(float[] queryVector, int limit = 5, float minSimilarity = 0.7f)
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$vectorSearch", new BsonDocument
            {
                { "index", "vector_index" },
                { "path", "Vector" },
                { "queryVector", new BsonArray(queryVector.Select(v => (BsonValue)v)) },
                { "numCandidates", 100 },
                { "limit", limit }
            })
        };

        var results = await _vectors.Aggregate<VectorEntry>(pipeline).ToListAsync();

        return results
            .Select(r =>
            {
                float similarity = CosineSimilarity(queryVector, r.Vector);
                return (r.Id, r.Text, similarity);
            })
            .Where(r => r.similarity >= minSimilarity)
            .OrderByDescending(r => r.similarity)
            .Take(limit)
            .ToList();
    }

    public async Task DeleteVectorAsync(Guid id)
    {
        await _vectors.DeleteOneAsync(v => v.Id == id);
    }

    private float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        float dotProduct = 0;
        float magnitudeA = 0;
        float magnitudeB = 0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        return dotProduct / (float)(Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}

public class VectorEntry
{
    public Guid Id { get; set; }
    public float[] Vector { get; set; }
    public string Text { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
}