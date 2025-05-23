namespace ChatLendGo.Application.DTOs;

public class ConversationDto
{
    public string Id { get; set; }
    public string TutorName { get; set; }
    public string Title { get; set; }
    public string LastMessagePreview { get; set; }
    public DateTime LastMessageTime { get; set; }
    public int MessageCount { get; set; }
}
