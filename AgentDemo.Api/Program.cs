using AgentDemo.Application.Services;
using AgentDemo.Core.Interfaces;
using AgentDemo.Domain.Entities;
using AgentDemo.Infrastructure.Repositories;
using AgentDemo.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<ISectionRepository, SectionRepository>();
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
builder.Services.AddSingleton<ISummaryService, SummaryService>();
builder.Services.AddSingleton<IAgentInsightService, AgentInsightService>();
builder.Services.AddSingleton<SectionService>();
builder.Services.AddSingleton<ISummaryContextRepository, SummaryContextRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var scopedServices = scope.ServiceProvider;
    await SeedData(scopedServices);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

static async Task SeedData(IServiceProvider services)
{
    var sectionRepo = services.GetRequiredService<ISectionRepository>();
    var messageRepo = services.GetRequiredService<IMessageRepository>();

    var sectionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    if ((await sectionRepo.GetByIdAsync(sectionId)) == null)
    {
        await sectionRepo.AddAsync(new Section
        {
            Id = sectionId,
            Title = "Test Section",
            Path = "/doc/1"
        });

        var messages = new[]
        {
            new Message
            {
                Id = Guid.NewGuid(),
                SectionId = sectionId,
                Author = "Alice",
                Content = "This section needs a better summary.",
                Timestamp = DateTime.UtcNow.AddMinutes(-5)
            },
            new Message
            {
                Id = Guid.NewGuid(),
                SectionId = sectionId,
                Author = "Bob",
                Content = "Agreed. Want me to take a stab at it?",
                Timestamp = DateTime.UtcNow.AddMinutes(-4)
            },
            new Message
            {
                Id = Guid.NewGuid(),
                SectionId = sectionId,
                Author = "Alice",
                Content = "Yes please. Focus on the compliance angle.",
                Timestamp = DateTime.UtcNow.AddMinutes(-3)
            }
        };

        foreach (var msg in messages)
            await messageRepo.AddAsync(msg);

        var sectionService = services.GetRequiredService<SectionService>();
        await sectionService.AddMessageAsync(sectionId, new Message
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Author = "Seeder",
            Content = "(initial trigger)",
            Timestamp = DateTime.UtcNow
        });
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}