
public class AttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set;} = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public string FileUrl {get; set; } = string.Empty; // URL para descargar el archivo
    public string StoredFileName { get; set; } = string.Empty; // Nombre fisico en disco.
    public int IncidentId { get; set; }
}