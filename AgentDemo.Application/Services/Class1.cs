using AgentDemo.Core.Interfaces;
using AgentDemo.Domain.Entities;

namespace AgentDemo.Application.Services;

public class SectionService
{
    private readonly IAgentInsightService _insightService;
    private readonly IMessageRepository _messageRepo;
    private readonly ISectionRepository _sectionRepo;
    private readonly ISummaryService _summaryService;

    public SectionService(
        ISectionRepository sectionRepo,
        IMessageRepository messageRepo,
        ISummaryService summaryService,
        IAgentInsightService insightService)
    {
        _sectionRepo = sectionRepo;
        _messageRepo = messageRepo;
        _summaryService = summaryService;
        _insightService = insightService;
    }

    public async Task AddMessageAsync(Guid sectionId, Message message)
    {
        var section = await _sectionRepo.GetByIdAsync(sectionId);
        if (section == null) throw new Exception("Section not found");

        message.SectionId = sectionId;
        await _messageRepo.AddAsync(message);

        var messages = await _messageRepo.GetBySectionIdAsync(sectionId);
        var summary = await _summaryService.GenerateSummaryAsync(section, messages, null);
        var insights = await _insightService.ExtractInsightsAsync(summary);

        // store summary/insights if persistence is added later
        section.Messages = messages.ToList();
        await _sectionRepo.UpdateAsync(section);
    }

    public async Task CreateSectionAsync(Section section)
    {
        await _sectionRepo.AddAsync(section);
    }

    public async Task<IEnumerable<Section>> GetAllSectionsAsync()
    {
        return await _sectionRepo.GetAllAsync();
    }

    public async Task<IEnumerable<AgentInsight>> GetInsightsAsync(Guid sectionId)
    {
        var section = await _sectionRepo.GetByIdAsync(sectionId);
        if (section == null) throw new Exception("Section not found");

        var messages = await _messageRepo.GetBySectionIdAsync(sectionId);
        var summary = await _summaryService.GenerateSummaryAsync(section, messages, null);
        return await _insightService.ExtractInsightsAsync(summary);
    }

    public async Task<IEnumerable<Message>> GetMessagesBySectionIdAsync(Guid sectionId)
    {
        return await _messageRepo.GetBySectionIdAsync(sectionId);
    }

    public async Task<Section?> GetSectionByIdAsync(Guid id)
    {
        return await _sectionRepo.GetByIdAsync(id);
    }

    public async Task<Summary> GetSummaryAsync(Guid sectionId)
    {
        var section = await _sectionRepo.GetByIdAsync(sectionId);
        if (section == null) throw new Exception("Section not found");

        var messages = await _messageRepo.GetBySectionIdAsync(sectionId);
        return await _summaryService.GenerateSummaryAsync(section, messages, null);
    }
}