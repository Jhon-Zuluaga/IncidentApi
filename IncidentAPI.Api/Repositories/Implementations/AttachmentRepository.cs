

using IncidentAPI.Api.Data;
using Microsoft.EntityFrameworkCore;

public class AttachmentRepository : IAttachmentRepository
{
   private readonly AppDbContext _context;

   public AttachmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Attachment>> GetAllByIncidentIdAsync(int incidentId)
    {
        return await _context.Attachments
            .Where(a => a.IncidentId == incidentId)
            .ToListAsync();
    }

    public async Task<Attachment?> GetByIdAsync(int id)
    {
        return await _context.Attachments
            .Include(a => a.Incident)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Attachment> CreateAsync(Attachment attachment)
    {
        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync();
        return attachment;
    }   

    public async Task<bool> DeleteAsync(int id)
    {
        var attachment = await _context.Attachments.FindAsync(id);
        if(attachment == null) return false;

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();
        return true;
    }
}