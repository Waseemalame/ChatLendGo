namespace ChatLendGo.Domain.Entities;

using ChatLendGo.Domain.Enums;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid TutorId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastMessageAt { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
    public ModelType ModelType { get; set; }
}
