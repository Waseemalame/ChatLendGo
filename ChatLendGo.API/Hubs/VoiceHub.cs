using System.Threading.Tasks;
using ChatLendGo.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatLendGo.API.Hubs;

public class VoiceHub : Hub
{
    private readonly IVoiceInteractionService _voiceService;

    public VoiceHub(IVoiceInteractionService voiceService)
    {
        _voiceService = voiceService;
    }

    public async Task StartStreaming(string conversationId)
    {
        await _voiceService.InitializeStreamingSessionAsync(conversationId);
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task StreamAudio(string sessionId, byte[] audioChunk)
    {
        await _voiceService.ProcessAudioChunkAsync(sessionId, audioChunk);
    }

    public async Task EndStreaming(string sessionId)
    {
        await _voiceService.EndStreamingSessionAsync(sessionId);
    }
}