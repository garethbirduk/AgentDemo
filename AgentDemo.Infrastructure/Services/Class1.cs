using AgentDemo.Core.Interfaces;
using AgentDemo.Domain.Entities;

namespace AgentDemo.Infrastructure.Services;

public class AgentInsightService : IAgentInsightService
{
    public Task<IEnumerable<AgentInsight>> ExtractInsightsAsync(Summary summary)
    {
        var insights = new List<AgentInsight>();
        var lines = summary.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var lower = line.ToLowerInvariant();
            var type = InsightType.Comment;

            if (lower.Contains("do") || lower.Contains("complete") || lower.Contains("take"))
                type = InsightType.Task;
            else if (lower.Contains("?"))
                type = InsightType.Question;
            else if (lower.Contains("decide") || lower.Contains("approved"))
                type = InsightType.Decision;
            else if (lower.Contains("good job") || lower.Contains("like this"))
                type = InsightType.Approval;
            else if (lower.Contains("disagree") || lower.Contains("wrong"))
                type = InsightType.Objection;

            insights.Add(new AgentInsight
            {
                Id = Guid.NewGuid(),
                SectionId = summary.SectionId,
                Type = type,
                Content = line.Trim()
            });
        }

        return Task.FromResult(insights.AsEnumerable());
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