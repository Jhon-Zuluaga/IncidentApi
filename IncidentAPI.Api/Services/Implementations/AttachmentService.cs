
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;

public class AttachmentService(
    IAttachmentRepository attachmentRepository,
    IIncidentRepository incidentRepository,
    IWebHostEnvironment env) : IAttachmentService
{
    private static readonly string[] _allowedTyped =
        { "image/jpeg", "image/png", "image/gif", "application/pdf", "text/plain"};
    private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

    public async Task<IEnumerable<AttachmentDto>> GetAllByIncidentIdAsync (int incidentId, int userId)
    {
        var incident = await incidentRepository.GetByIdAsync(incidentId);
        if(incident is null)
            throw new NotFoundException($"Incident with ID {incidentId} not found.");
        
        if(incident.UserId != userId)
            throw new ForbiddenException("You do not have permission to view the attachments for this incident.");

        var attachments = await attachmentRepository.GetAllByIncidentIdAsync(incidentId);
        return attachments. Select(a => ToDto(a));
    }

    public async Task<AttachmentDto?> GetByIdAsync(int id, int userId)
    {
        var attachment = await attachmentRepository.GetByIdAsync(id);
        if(attachment is null) return null;

        if(attachment.Incident.UserId != userId)
            throw new ForbiddenException("You do not have permission to access this attachment.");
        
        return ToDto(attachment);
    }

    public async Task<AttachmentDto?> UploadAsync(int incidentId, CreateAttachmentDto dto, string baseUrl, int userId)
    {
        // Verificar que el incidente existe
        var incident = await incidentRepository.GetByIdAsync(incidentId);
        if(incident is null) return null;

        if(incident.UserId != userId)
            throw new ForbiddenException("You do not have permission to upload files to this incident.");

        // Validar tipo de archivo
        if(!_allowedTyped.Contains(dto.File.ContentType)) return null;

        // Validar tamaño
        if(dto.File.Length > _maxFileSize) return null;

        // Generar nombre unico para no sobreescribir archivos con el mismo nombre
        var extension = Path.GetExtension(dto.File.FileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";

        // Crear la carpeta uploads si no existe
        var uploadsFolder = Path.Combine(env.ContentRootPath, "App_Data", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        // Guardar archivo en disco
        var filePath = Path.Combine(uploadsFolder, storedFileName);
        using(var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        // Guardar el registro en la base de datos
        var attachment = new Attachment
        {
            FileName = dto.File.FileName,
            StoredFileName = storedFileName,
            FilePath = filePath,
            ContentType = dto.File.ContentType,
            FileSize = dto.File.Length,
            IncidentId = incidentId
        };

        var created = await attachmentRepository.CreateAsync(attachment);
        return ToDto(created, baseUrl);
    }

    
    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var attachment = await attachmentRepository.GetByIdAsync(id);
        if(attachment is null) return false;

        if(attachment.Incident.UserId != userId)
            throw new ForbiddenException("You do not have permission to delete this attachment.");

        // Eliminar el archivo fisico del disco.
        if(File.Exists(attachment.FilePath))
            File.Delete(attachment.FilePath);
        
        return await attachmentRepository.DeleteAsync(id);
    }

    public static AttachmentDto ToDto(Attachment attachment, string baseUrl = "")
    {
        return new AttachmentDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize,
            UploadedAt = attachment.UploadedAt,
            IncidentId = attachment.IncidentId,
            StoredFileName = attachment.StoredFileName,
            FileUrl = string.IsNullOrEmpty(baseUrl)
                ? attachment.StoredFileName
                : $"{baseUrl}/api/attachment/{attachment.Id}/download"
        };
    }
}