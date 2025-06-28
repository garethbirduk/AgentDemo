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