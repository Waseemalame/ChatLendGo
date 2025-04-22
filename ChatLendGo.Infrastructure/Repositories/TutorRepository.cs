using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatLendGo.Domain.Entities;
using ChatLendGo.Domain.Interfaces;
using ChatLendGo.Infrastructure.Data;
using MongoDB.Driver;

namespace ChatLendGo.Infrastructure.Repositories;

public class TutorRepository : ITutorRepository
{
    private readonly IMongoCollection<Tutor> _tutors;

    public TutorRepository(MongoDbContext context)
    {
        _tutors = context.GetCollection<Tutor>("tutors");
    }

    public async Task<List<Tutor>> GetAllTutorsAsync()
    {
        return await _tutors.Find(_ => true).ToListAsync();
    }

    public async Task<Tutor> GetTutorByIdAsync(Guid id)
    {
        return await _tutors.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Tutor> CreateTutorAsync(Tutor tutor)
    {
        await _tutors.InsertOneAsync(tutor);
        return tutor;
    }

    public async Task UpdateTutorAsync(Tutor tutor)
    {
        await _tutors.ReplaceOneAsync(t => t.Id == tutor.Id, tutor);
    }

    public async Task DeleteTutorAsync(Guid id)
    {
        await _tutors.DeleteOneAsync(t => t.Id == id);
    }
}