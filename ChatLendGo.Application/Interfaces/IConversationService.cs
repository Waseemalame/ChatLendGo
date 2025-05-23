using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatLendGo.Application.DTOs;
using ChatLendGo.Domain.Enums;

namespace ChatLendGo.Application.Interfaces;

public interface IConversationService
{
    Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(string userId);
    Task<ConversationDto> GetConversationByIdAsync(string id);
    Task<ConversationDto> CreateConversationAsync(string userId, string tutorId, ModelType modelType);
    Task<MessageDto> SendMessageAsync(string conversationId, string message, bool isFromUser);
    Task DeleteConversationAsync(string id);
}