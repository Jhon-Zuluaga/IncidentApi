

public interface IAttachmentService
{
    Task<IEnumerable<AttachmentDto>> GetAllByIncidentIdAsync(int incidentId, int userId);
    Task<AttachmentDto?> GetByIdAsync(int id, int userId);
    Task<AttachmentDto?> UploadAsync(int incidentId, CreateAttachmentDto dto, string baseUrl, int userId); 
    Task<bool> DeleteAsync(int id, int userId);
}