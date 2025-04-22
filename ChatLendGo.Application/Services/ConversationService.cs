using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatLendGo.Application.DTOs;
using ChatLendGo.Application.Interfaces;
using ChatLendGo.Domain.Entities;
using ChatLendGo.Domain.Interfaces;
using ChatLendGo.Domain.Enums;

namespace ChatLendGo.Application.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ITutorRepository _tutorRepository;
    private readonly ILlmProvider _llmProvider;
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly IVectorRepository _vectorRepository;

    public ConversationService(
        IConversationRepository conversationRepository,
        ITutorRepository tutorRepository,
        ILlmProvider llmProvider,
        IEmbeddingProvider embeddingProvider,
        IVectorRepository vectorRepository)
    {
        _conversationRepository = conversationRepository;
        _tutorRepository = tutorRepository;
        _llmProvider = llmProvider;
        _embeddingProvider = embeddingProvider;
        _vectorRepository = vectorRepository;
    }

    public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(string userId)
    {
        var conversations = await _conversationRepository.GetUserConversationsAsync(Guid.Parse(userId));
        var result = new List<ConversationDto>();

        foreach (var conversation in conversations)
        {
            var tutor = await _tutorRepository.GetTutorByIdAsync(conversation.TutorId);
            result.Add(MapToDto(conversation, tutor.Name));
        }

        return result.OrderByDescending(c => c.LastMessageTime);
    }

    public async Task<ConversationDto> GetConversationByIdAsync(string id)
    {
        var conversation = await _conversationRepository.GetConversationByIdAsync(Guid.Parse(id));
        if (conversation == null)
            return null;

        var tutor = await _tutorRepository.GetTutorByIdAsync(conversation.TutorId);
        return MapToDto(conversation, tutor.Name);
    }

    public async Task<ConversationDto> CreateConversationAsync(string userId, string tutorId, ModelType modelType)
    {
        var tutor = await _tutorRepository.GetTutorByIdAsync(Guid.Parse(tutorId));
        if (tutor == null)
            throw new KeyNotFoundException($"Tutor with ID {tutorId} not found");

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            TutorId = tutor.Id,
            UserId = Guid.Parse(userId),
            Title = $"Conversation with {tutor.Name}",
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow,
            Messages = new List<Message>(),
            ModelType = modelType
        };

        var createdConversation = await _conversationRepository.CreateConversationAsync(conversation);
        return MapToDto(createdConversation, tutor.Name);
    }

    public async Task<MessageDto> SendMessageAsync(string conversationId, string content, bool isFromUser)
    {
        var conversation = await _conversationRepository.GetConversationByIdAsync(Guid.Parse(conversationId));
        if (conversation == null)
            throw new KeyNotFoundException($"Conversation with ID {conversationId} not found");

        var tutor = await _tutorRepository.GetTutorByIdAsync(conversation.TutorId);

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Content = content,
            IsFromUser = isFromUser,
            Timestamp = DateTime.UtcNow
        };

        if (isFromUser)
        {
            await _conversationRepository.AddMessageToConversationAsync(conversation.Id, message);

            var userMessage = message;
            var tutorResponse = await GenerateTutorResponseAsync(conversation, tutor, userMessage);

            return MapToMessageDto(tutorResponse);
        }
        else
        {
            await _conversationRepository.AddMessageToConversationAsync(conversation.Id, message);
            return MapToMessageDto(message);
        }
    }

    public async Task DeleteConversationAsync(string id)
    {
        await _conversationRepository.DeleteConversationAsync(Guid.Parse(id));
    }

    private async Task<Message> GenerateTutorResponseAsync(Conversation conversation, Tutor tutor, Message userMessage)
    {
        var chatHistory = conversation.Messages.Select(m => (m.Content, m.IsFromUser)).ToList();
        chatHistory.Add((userMessage.Content, true));

        string response = await _llmProvider.GenerateResponseAsync(
            chatHistory,
            tutor.SystemPrompt);

        var responseMessage = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Content = response,
            IsFromUser = false,
            Timestamp = DateTime.UtcNow
        };

        await _conversationRepository.AddMessageToConversationAsync(conversation.Id, responseMessage);

        if (!string.IsNullOrEmpty(response))
        {
            var embedding = await _embeddingProvider.GenerateEmbeddingAsync(response);
            var embeddingId = await _vectorRepository.StoreVectorAsync(embedding, response, responseMessage.Id);
            responseMessage.EmbeddingId = embeddingId;

            conversation.LastMessageAt = DateTime.UtcNow;
            await _conversationRepository.UpdateConversationAsync(conversation);
        }

        return responseMessage;
    }

    private ConversationDto MapToDto(Conversation conversation, string tutorName)
    {
        var lastMessage = conversation.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault();
        string lastMessagePreview = lastMessage != null
            ? (lastMessage.Content.Length > 50
                ? lastMessage.Content.Substring(0, 47) + "..."
                : lastMessage.Content)
            : string.Empty;

        return new ConversationDto
        {
            Id = conversation.Id.ToString(),
            TutorName = tutorName,
            Title = conversation.Title,
            LastMessagePreview = lastMessagePreview,
            LastMessageTime = lastMessage?.Timestamp ?? conversation.CreatedAt,
            MessageCount = conversation.Messages.Count
        };
    }

    private MessageDto MapToMessageDto(Message message)
    {
        return new MessageDto
        {
            Id = message.Id.ToString(),
            Content = message.Content,
            IsFromUser = message.IsFromUser,
            Timestamp = message.Timestamp
        };
    }
}