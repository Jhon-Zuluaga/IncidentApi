

using IncidentAPI.Api.Data;
using Microsoft.EntityFrameworkCore;

public class AttachmentRepository(AppDbContext context) : IAttachmentRepository
{

    public async Task<IEnumerable<Attachment>> GetAllByIncidentIdAsync(int incidentId)
    {
        return await context.Attachments
            .Where(a => a.IncidentId == incidentId)
            .ToListAsync();
    }

    public async Task<Attachment?> GetByIdAsync(int id)
    {
        return await context.Attachments
            .Include(a => a.Incident)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Attachment> CreateAsync(Attachment attachment)
    {
        context.Attachments.Add(attachment);
        await context.SaveChangesAsync();
        return attachment;
    }   

    public async Task<bool> DeleteAsync(int id)
    {
        var attachment = await context.Attachments.FindAsync(id);
        if(attachment == null) return false;

        context.Attachments.Remove(attachment);
        await context.SaveChangesAsync();
        return true;
    }
}