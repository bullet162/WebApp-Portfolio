using BlazorPortfolio.Data;
using BlazorPortfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorPortfolio.Services;

public class ContentService(IDbContextFactory<AppDbContext> factory)
{
    // ── Experiences ──────────────────────────────────────────────────────────
    public async Task<List<Experience>> GetExperiencesAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Experiences.OrderBy(e => e.SortOrder).ToListAsync();
    }

    public async Task SaveExperienceAsync(Experience exp)
    {
        await using var db = await factory.CreateDbContextAsync();
        if (exp.Id == 0) db.Experiences.Add(exp);
        else db.Experiences.Update(exp);
        await db.SaveChangesAsync();
    }

    public async Task DeleteExperienceAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.Experiences.Where(e => e.Id == id).ExecuteDeleteAsync();
    }

    // ── Skills ───────────────────────────────────────────────────────────────
    public async Task<List<Skill>> GetSkillsAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Skills.OrderBy(s => s.Category).ThenBy(s => s.Name).ToListAsync();
    }

    public async Task SaveSkillAsync(Skill skill)
    {
        await using var db = await factory.CreateDbContextAsync();
        if (skill.Id == 0) db.Skills.Add(skill);
        else db.Skills.Update(skill);
        await db.SaveChangesAsync();
    }

    public async Task DeleteSkillAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.Skills.Where(s => s.Id == id).ExecuteDeleteAsync();
    }

    // ── Projects ─────────────────────────────────────────────────────────────
    public async Task<List<Project>> GetProjectsAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Projects.OrderBy(p => p.SortOrder).ToListAsync();
    }

    public async Task SaveProjectAsync(Project project)
    {
        await using var db = await factory.CreateDbContextAsync();
        if (project.Id == 0) db.Projects.Add(project);
        else db.Projects.Update(project);
        await db.SaveChangesAsync();
    }

    public async Task DeleteProjectAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.Projects.Where(p => p.Id == id).ExecuteDeleteAsync();
    }

    // ── Site Profile ──────────────────────────────────────────────────────────
    public async Task<SiteProfile> GetProfileAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.SiteProfiles.FirstOrDefaultAsync()
               ?? new SiteProfile();
    }

    public async Task SaveProfileAsync(SiteProfile profile)
    {
        await using var db = await factory.CreateDbContextAsync();
        if (profile.Id == 0) db.SiteProfiles.Add(profile);
        else db.SiteProfiles.Update(profile);
        await db.SaveChangesAsync();
    }

    // ── Hiring Messages ───────────────────────────────────────────────────────
    public async Task<List<HiringMessage>> GetMessagesAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.HiringMessages.OrderByDescending(m => m.SentAt).ToListAsync();
    }

    public async Task AddMessageAsync(HiringMessage msg)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.HiringMessages.Add(msg);
        await db.SaveChangesAsync();
    }

    public async Task MarkReadAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.HiringMessages.Where(m => m.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true));
    }

    public async Task DeleteMessageAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.HiringMessages.Where(m => m.Id == id).ExecuteDeleteAsync();
    }
}
