using AgentDemo.Domain.Entities;

namespace AgentDemo.Core.Interfaces;

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
}

public interface IActionItemService
{
    Task<IEnumerable<ActionItem>> ExtractActionsAsync(Summary summary);
}