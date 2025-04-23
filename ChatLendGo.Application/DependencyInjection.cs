using ChatLendGo.Application.Interfaces;
using ChatLendGo.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatLendGo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IModelSelectionService, ModelSelectionService>();
        services.AddScoped<IVoiceInteractionService, VoiceInteractionService>();

        return services;
    }
}