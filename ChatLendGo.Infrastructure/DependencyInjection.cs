using System;
using System.Net.Http;
using ChatLendGo.Domain.Interfaces;
using ChatLendGo.Domain.Enums;
using ChatLendGo.Infrastructure.Data;
using ChatLendGo.Infrastructure.Repositories;
using ChatLendGo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatLendGo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        services.Configure<MongoDbSettings>(options =>
        {
            options.ConnectionString = configuration.GetSection("MongoDbSettings:ConnectionString").Value;
            options.DatabaseName = configuration.GetSection("MongoDbSettings:DatabaseName").Value;
        });
        services.AddSingleton<MongoDbContext>();

        // Repositories
        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IVectorRepository, VectorRepository>();

        // HTTP Clients
        services.AddHttpClient<OpenAiService>();
        services.AddHttpClient<AnthropicService>();
        services.AddHttpClient<OpenAiEmbeddingService>();
        services.AddHttpClient<WhisperService>();

        // LLM Services
        services.Configure<OpenAiSettings>(options =>
        {
            options.ApiKey = configuration.GetSection("OpenAiSettings:ApiKey").Value;
            options.ApiEndpoint = configuration.GetSection("OpenAiSettings:ApiEndpoint").Value;
            options.Model = configuration.GetSection("OpenAiSettings:Model").Value;
        });

        services.Configure<AnthropicSettings>(options =>
        {
            options.ApiKey = configuration.GetSection("AnthropicSettings:ApiKey").Value;
            options.ApiEndpoint = configuration.GetSection("AnthropicSettings:ApiEndpoint").Value;
            options.Model = configuration.GetSection("AnthropicSettings:Model").Value;
        });

        // Register LLM providers with keyed services
        services.AddKeyedScoped<ILlmProvider, OpenAiService>(ModelType.OpenAI);
        services.AddKeyedScoped<ILlmProvider, AnthropicService>(ModelType.Anthropic);

        // Register LLM provider factory
        services.AddScoped<Func<ModelType, ILlmProvider>>(serviceProvider => key =>
            serviceProvider.GetKeyedService<ILlmProvider>(key));

        // Embedding Service
        services.Configure<OpenAiEmbeddingSettings>(options =>
        {
            options.ApiKey = configuration.GetSection("OpenAiEmbeddingSettings:ApiKey").Value;
            options.ApiEndpoint = configuration.GetSection("OpenAiEmbeddingSettings:ApiEndpoint").Value;
            options.Model = configuration.GetSection("OpenAiEmbeddingSettings:Model").Value;
        });
        services.AddScoped<IEmbeddingProvider, OpenAiEmbeddingService>();

        // Speech Services
        services.Configure<WhisperSettings>(options =>
        {
            options.ApiKey = configuration.GetSection("WhisperSettings:ApiKey").Value;
            options.ApiEndpoint = configuration.GetSection("WhisperSettings:ApiEndpoint").Value;
            options.Model = configuration.GetSection("WhisperSettings:Model").Value;
        });

        services.Configure<PiperSettings>(options =>
        {
            options.PiperExecutablePath = configuration.GetSection("PiperSettings:PiperExecutablePath").Value;
            options.ModelPath = configuration.GetSection("PiperSettings:ModelPath").Value;
        });

        services.AddScoped<WhisperService>();
        services.AddScoped<PiperTtsService>();

        return services;
    }
}