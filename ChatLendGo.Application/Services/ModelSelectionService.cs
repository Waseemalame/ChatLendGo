using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatLendGo.Application.Interfaces;
using ChatLendGo.Domain.Enums;
using ChatLendGo.Domain.Interfaces;

namespace ChatLendGo.Application.Services;

public class ModelSelectionService : IModelSelectionService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly Dictionary<ModelType, string> _modelNames;

    public ModelSelectionService(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
        _modelNames = new Dictionary<ModelType, string>
        {
            { ModelType.OpenAI, "GPT-4 Turbo" },
            { ModelType.Anthropic, "Claude 3 Opus" },
            { ModelType.Llama, "Llama 3 70B" },
            { ModelType.Mistral, "Mistral Large" }
        };
    }

    public async Task<IEnumerable<string>> GetAvailableModelsAsync()
    {
        return _modelNames.Values;
    }

    public async Task ChangeModelForConversationAsync(string conversationId, ModelType modelType)
    {
        var conversation = await _conversationRepository.GetConversationByIdAsync(Guid.Parse(conversationId));
        if (conversation == null)
            throw new KeyNotFoundException($"Conversation with ID {conversationId} not found");

        conversation.ModelType = modelType;
        await _conversationRepository.UpdateConversationAsync(conversation);
    }

    public ModelType GetDefaultModelType()
    {
        return ModelType.OpenAI;
    }
}