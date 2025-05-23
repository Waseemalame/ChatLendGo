using ChatLendGo.Domain.Entities;

namespace ChatLendGo.Domain.Interfaces;

public interface IConversationRepository
{
    Task<List<Conversation>> GetUserConversationsAsync(Guid userId);
    Task<Conversation> GetConversationByIdAsync(Guid id);
    Task<Conversation> CreateConversationAsync(Conversation conversation);
    Task AddMessageToConversationAsync(Guid conversationId, Message message);
    Task UpdateConversationAsync(Conversation conversation);
    Task DeleteConversationAsync(Guid id);
}
