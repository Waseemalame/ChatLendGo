namespace ChatLendGo.Domain.Entities;

using System;
using System.Collections.Generic;
using ChatLendGo.Domain.Enums;

public class Tutor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public TutorSpecialty Specialty { get; set; }
    public string SystemPrompt { get; set; }
    public List<string> KnowledgeAreas { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
