namespace ChatLendGo.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public bool IsFromUser { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? EmbeddingId { get; set; }
}
