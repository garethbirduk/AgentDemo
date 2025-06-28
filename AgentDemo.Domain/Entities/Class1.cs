namespace AgentDemo.Domain.Entities;

public enum InsightType
{
    Comment,
    Task,
    Question,
    Decision,
    Approval,
    Objection,
    Rating,
    LinkReference,
    UploadReference
}

public class AgentInsight
{
    public string? Author { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public Guid SectionId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public InsightType Type { get; set; }
    // optional key-value tags
}

public class Message
{
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class Section
{
    public Guid Id { get; set; }
    public List<Message> Messages { get; set; } = new();
    public string? Path { get; set; }
    public string Title { get; set; } = string.Empty;
    // optional: XPath or identifier in doc
}

public class Summary
{
    public string Content { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public Guid SectionId { get; set; }
}

public class SummaryContext
{
    public Guid Id { get; set; }
    public List<string> KeyPoints { get; set; } = new();
    public Guid? LastMessageId { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public Guid SectionId { get; set; }
    public string SummaryText { get; set; } = string.Empty;
    public List<string> UnresolvedTasks { get; set; } = new();
}