namespace ChatLendGo.Application.DTOs;

public class MessageDto
{
    public string Id { get; set; }
    public string Content { get; set; }
    public bool IsFromUser { get; set; }
    public DateTime Timestamp { get; set; }
}
