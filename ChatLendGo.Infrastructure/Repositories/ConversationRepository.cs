using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatLendGo.Domain.Entities;
using ChatLendGo.Domain.Interfaces;
using ChatLendGo.Infrastructure.Data;
using MongoDB.Driver;

namespace ChatLendGo.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly IMongoCollection<Conversation> _conversations;

    public ConversationRepository(MongoDbContext context)
    {
        _conversations = context.GetCollection<Conversation>("conversations");
    }

    public async Task<List<Conversation>> GetUserConversationsAsync(Guid userId)
    {
        return await _conversations.Find(c => c.UserId == userId).ToListAsync();
    }

    public async Task<Conversation> GetConversationByIdAsync(Guid id)
    {
        return await _conversations.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Conversation> CreateConversationAsync(Conversation conversation)
    {
        await _conversations.InsertOneAsync(conversation);
        return conversation;
    }

    public async Task AddMessageToConversationAsync(Guid conversationId, Message message)
    {
        var update = Builders<Conversation>.Update.Push(c => c.Messages, message)
            .Set(c => c.LastMessageAt, DateTime.UtcNow);

        await _conversations.UpdateOneAsync(c => c.Id == conversationId, update);
    }

    public async Task UpdateConversationAsync(Conversation conversation)
    {
        await _conversations.ReplaceOneAsync(c => c.Id == conversation.Id, conversation);
    }

    public async Task DeleteConversationAsync(Guid id)
    {
        await _conversations.DeleteOneAsync(c => c.Id == id);
    }
}