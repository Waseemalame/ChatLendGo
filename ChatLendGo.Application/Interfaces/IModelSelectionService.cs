using System.Collections.Generic;
using System.Threading.Tasks;
using ChatLendGo.Domain.Enums;

namespace ChatLendGo.Application.Interfaces;

public interface IModelSelectionService
{
    Task<IEnumerable<string>> GetAvailableModelsAsync();
    Task ChangeModelForConversationAsync(string conversationId, ModelType modelType);
    ModelType GetDefaultModelType();
}