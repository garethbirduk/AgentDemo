using AgentDemo.Core.Interfaces;
using AgentDemo.Domain.Entities;

namespace AgentDemo.Application.Services;

public class SectionService
{
    private readonly IAgentInsightService _insightService;
    private readonly IMessageRepository _messageRepo;
    private readonly ISectionRepository _sectionRepo;
    private readonly ISummaryContextRepository _summaryContextRepository;
    private readonly ISummaryService _summaryService;

    public SectionService(
        ISectionRepository sectionRepo,
        IMessageRepository messageRepo,
        ISummaryService summaryService,
        IAgentInsightService insightService,
        ISummaryContextRepository summaryContextRepository)
    {
        _summaryContextRepository = summaryContextRepository;
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

        var previousContext = await _summaryContextRepository.GetBySectionIdAsync(sectionId);

        var allMessages = await _messageRepo.GetBySectionIdAsync(sectionId);
        IEnumerable<Message> newMessages;

        if (previousContext?.LastMessageId is Guid lastId)
        {
            var found = false;
            newMessages = new List<Message>();

            foreach (var msg in allMessages.OrderBy(m => m.Timestamp))
            {
                if (found)
                    ((List<Message>)newMessages).Add(msg);

                if (msg.Id == lastId)
                    found = true;
            }
        }
        else
        {
            newMessages = allMessages;
        }

        var context = await _summaryService.GenerateSummaryContextAsync(section, newMessages, previousContext);
        await _summaryContextRepository.SaveAsync(context);

        section.Messages = allMessages.ToList();
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

    public async Task<SummaryContext?> GetSummaryContextAsync(Guid sectionId)
    {
        return await _summaryContextRepository.GetBySectionIdAsync(sectionId);
    }
}