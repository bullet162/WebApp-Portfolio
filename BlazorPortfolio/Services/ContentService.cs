using BlazorPortfolio.Data;
using BlazorPortfolio.Models;
using BlazorPortfolio.Services;
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
        return await db.SiteProfiles.OrderBy(s => s.Id).FirstOrDefaultAsync()
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

    // ── Collaboration Requests ────────────────────────────────────────────────
    public async Task<List<CollaborationRequest>> GetCollaborationRequestsAsync(CollaborationStatus? status = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        var q = db.CollaborationRequests.AsQueryable();
        if (status.HasValue) q = q.Where(r => r.Status == status.Value);
        return await q.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task AddCollaborationRequestAsync(CollaborationRequest req)
    {
        req.Status = SpamDetectionService.Evaluate(req);
        await using var db = await factory.CreateDbContextAsync();
        db.CollaborationRequests.Add(req);
        await db.SaveChangesAsync();
    }

    public async Task SetCollaborationStatusAsync(int id, CollaborationStatus newStatus)
    {
        await using var db = await factory.CreateDbContextAsync();
        var req = await db.CollaborationRequests.FindAsync(id);
        if (req is null) return;

        // Enforce valid transitions
        bool allowed = (req.Status, newStatus) switch
        {
            (CollaborationStatus.Pending,  CollaborationStatus.Approved) => true,
            (CollaborationStatus.Pending,  CollaborationStatus.Rejected) => true,
            (CollaborationStatus.Pending,  CollaborationStatus.Flagged)  => true,
            (CollaborationStatus.Flagged,  CollaborationStatus.Approved) => true,
            (CollaborationStatus.Flagged,  CollaborationStatus.Rejected) => true,
            (CollaborationStatus.Flagged,  CollaborationStatus.Pending)  => true,
            _ => false
        };

        if (!allowed) return;

        req.Status = newStatus;
        req.ReviewedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task DeleteCollaborationRequestAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        await db.CollaborationRequests.Where(r => r.Id == id).ExecuteDeleteAsync();
    }

    public async Task<List<CollaborationRequest>> GetApprovedCollaboratorsAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        var approved = await db.CollaborationRequests
            .Where(r => r.Status == CollaborationStatus.Approved)
            .ToListAsync();

        // Sort by completeness score: portfolio (+1) + message (+1) = max 2
        return approved
            .OrderByDescending(r => (string.IsNullOrEmpty(r.PortfolioUrl) ? 0 : 1)
                                  + (string.IsNullOrEmpty(r.Message) ? 0 : 1))
            .ThenByDescending(r => r.ReviewedAt)
            .ToList();
    }
}
