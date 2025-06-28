using AgentDemo.Application.Services;
using AgentDemo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AgentDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SectionController : ControllerBase
{
    private readonly SectionService _sectionService;

    public SectionController(SectionService sectionService)
    {
        _sectionService = sectionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSection([FromBody] Section section)
    {
        section.Id = Guid.NewGuid();
        await _sectionService.CreateSectionAsync(section);
        return Ok(section);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSections()
    {
        var sections = await _sectionService.GetAllSectionsAsync();
        return Ok(sections);
    }

    [HttpGet("{sectionId}/insights")]
    public async Task<IActionResult> GetInsights(Guid sectionId)
    {
        var insights = await _sectionService.GetInsightsAsync(sectionId);
        return Ok(insights);
    }

    [HttpGet("{sectionId}/messages")]
    public async Task<IActionResult> GetMessages(Guid sectionId)
    {
        var messages = await _sectionService.GetMessagesBySectionIdAsync(sectionId);
        return Ok(messages);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSectionById(Guid id)
    {
        var section = await _sectionService.GetSectionByIdAsync(id);
        if (section == null) return NotFound();
        return Ok(section);
    }

    [HttpGet("{sectionId}/summary")]
    public async Task<IActionResult> GetSummary(Guid sectionId)
    {
        var summary = await _sectionService.GetSummaryAsync(sectionId);
        return Ok(summary);
    }

    [HttpPost("{sectionId}/messages")]
    public async Task<IActionResult> PostMessage(Guid sectionId, [FromBody] Message message)
    {
        await _sectionService.AddMessageAsync(sectionId, message);
        return Ok();
    }
}