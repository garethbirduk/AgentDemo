using AgentDemo.Core.Interfaces;
using AgentDemo.Domain.Entities;

namespace AgentDemo.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly List<Message> _messages = new();

    public Task AddAsync(Message message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Message>> GetBySectionIdAsync(Guid sectionId)
    {
        var result = _messages.Where(m => m.SectionId == sectionId);
        return Task.FromResult(result);
    }
}

public class SectionRepository : ISectionRepository
{
    private readonly List<Section> _sections = new();

    public Task AddAsync(Section section)
    {
        _sections.Add(section);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _sections.RemoveAll(s => s.Id == id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Section>> GetAllAsync()
    {
        return Task.FromResult(_sections.AsEnumerable());
    }

    public Task<Section?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_sections.FirstOrDefault(s => s.Id == id));
    }

    public Task UpdateAsync(Section section)
    {
        var index = _sections.FindIndex(s => s.Id == section.Id);
        if (index != -1)
        {
            _sections[index] = section;
        }
        return Task.CompletedTask;
    }
}

public class SummaryService : ISummaryService
{
    public Task<Summary> GenerateSummaryAsync(Section section, IEnumerable<Message> newMessages, Summary? previousSummary)
    {
        var combinedText = string.Join("\n", newMessages.Select(m => m.Content));
        var summaryText = $"[AUTO-SUMMARY of {newMessages.Count()} messages]\n{combinedText}";

        var summary = new Summary
        {
            Id = previousSummary?.Id ?? Guid.NewGuid(),
            SectionId = section.Id,
            Content = summaryText,
            LastUpdated = DateTime.UtcNow
        };

        return Task.FromResult(summary);
    }
}

public class ActionItemService : IActionItemService
{
    public Task<IEnumerable<ActionItem>> ExtractActionsAsync(Summary summary)
    {
        // Dummy logic: extract lines with "do", "complete", "assign", etc.
        var keywords = new[] { "do", "complete", "assign", "review", "finish" };
        var actions = new List<ActionItem>();

        foreach (var line in summary.Content.Split('\n'))
        {
            if (keywords.Any(k => line.Contains(k, StringComparison.OrdinalIgnoreCase)))
            {
                actions.Add(new ActionItem
                {
                    Id = Guid.NewGuid(),
                    SectionId = summary.SectionId,
                    Description = line.Trim()
                });
            }
        }

        return Task.FromResult(actions.AsEnumerable());
    }
}