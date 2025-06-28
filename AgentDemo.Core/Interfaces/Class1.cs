using AgentDemo.Domain.Entities;

namespace AgentDemo.Core.Interfaces;

public interface IAgentInsightService
{
    Task<IEnumerable<AgentInsight>> ExtractInsightsAsync(Summary summary);
}

public interface IMessageRepository
{
    Task AddAsync(Message message);

    Task<IEnumerable<Message>> GetBySectionIdAsync(Guid sectionId);
}

public interface ISectionRepository
{
    Task AddAsync(Section section);

    Task DeleteAsync(Guid id);

    Task<IEnumerable<Section>> GetAllAsync();

    Task<Section?> GetByIdAsync(Guid id);

    Task UpdateAsync(Section section);
}

public interface ISummaryService
{
    Task<Summary> GenerateSummaryAsync(Section section, IEnumerable<Message> newMessages, Summary? previousSummary);

    Task<SummaryContext> GenerateSummaryContextAsync(
        Section section,
        IEnumerable<Message> newMessages,
        SummaryContext? previousContext
    );
}

public interface ISummaryContextRepository
{
    Task<SummaryContext?> GetBySectionIdAsync(Guid sectionId);

    Task SaveAsync(SummaryContext context);
}