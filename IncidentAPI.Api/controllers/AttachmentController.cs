
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttachmentController(
    IAttachmentService attachmentService, 
    IWebHostEnvironment env) : ControllerBase
{


    [HttpGet("incident/{incidentId}")]
    public async Task<IActionResult> GetAllByIncidentId(int incidentId)
    {
        var attachments = await attachmentService.GetAllByIncidentIdAsync(incidentId, User.GetUserId());

        if(!attachments.Any())
            return Ok("No hay archivos adjuntos para este incidente.");
        
        return Ok(attachments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var attachment = await attachmentService.GetByIdAsync(id, User.GetUserId());

        if(attachment is null)
            return NotFound("Archivo no encontrado.");

        return Ok(attachment);
    }

    [HttpPost("incident/{incidentId}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(int incidentId, [FromForm] CreateAttachmentDto dto)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var baseUrl = $"{Request.Scheme}: //{Request.Host}";
        var attachment = await attachmentService.UploadAsync(incidentId, dto, baseUrl, User.GetUserId());

        if(attachment is null)
            return BadRequest("No se pudo subir el archivo. Verifica que el incidente exista," + 
            "el tipo de archivo sea válido (jpg, png, gif, pdf, txt) " +
            "y que no supere los 5 MB.");
        
        return CreatedAtAction(nameof(GetById), new { id = attachment.Id }, attachment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await attachmentService.DeleteAsync(id, User.GetUserId());

        if(!deleted)
            return NotFound("Archivo no encontrado.");

        return Ok("Archivo eliminado correctamente");
    }
    

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var attachment = await attachmentService.GetByIdAsync(id, User.GetUserId());

        if(attachment is null)
            return NotFound("Archivo no encontrado.");
        
        // Bsucar archivo fisico en el disco
        var filePath = Path.Combine(env.ContentRootPath, "AppData", "uploads",
            attachment.FileUrl.Contains('/') ? attachment.FileUrl[(attachment.FileUrl.LastIndexOf('/') + 1)..] : attachment.FileUrl);

        if(!System.IO.File.Exists(filePath))
            return NotFound("El archivo ya no existe en el servidor.");

       return PhysicalFile(filePath, attachment.ContentType, attachment.FileName);
    }
}