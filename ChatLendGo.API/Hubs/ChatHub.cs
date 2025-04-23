using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatLendGo.API.Hubs;

public class ChatHub : Hub
{
    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendMessage(string conversationId, string message)
    {
        await Clients.Group(conversationId).SendAsync("ReceiveMessage", message);
    }

    public async Task TypingIndicator(string conversationId, bool isTyping)
    {
        await Clients.Group(conversationId).SendAsync("TypingStatus", isTyping);
    }
}