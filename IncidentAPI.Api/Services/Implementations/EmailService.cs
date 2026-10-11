using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

/*
    Este service:
        - Implementa el envio de correos usando SMTP
        - Lee la configuracion desde appsettings(host, puerto, credenciales)
        - Se usa para:
            * Notificar creacion de incidentes
            * Enviar tokens de recuperacion de contraseña
*/

// Implementacion del servicio de correos
public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;

    public EmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }
    
    // Crear y configurar el cliente SMTP
    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient
        {
            // Servidor SMTP(Gmail)
            Host = _emailOptions.Host,
            Port = _emailOptions.Port,

            // Habilita conexion segura (SSL)
            EnableSsl = true,

            // Credenciales del correo emisor
            Credentials = new NetworkCredential(
                _emailOptions.Username,
                _emailOptions.Password
            )
        };
    }

    // Crear estructura base del correo
    private MailMessage CreateBase(string toEmail, string toName, string subject)
    {
        var mail = new MailMessage();

        // Remitente (quien envia el correo)
        mail.From = new MailAddress(_emailOptions.From, _emailOptions.DisplayName);

        // Destinatario
        mail.To.Add(new MailAddress(toEmail, toName));
        
        // Asunto
        mail.Subject = subject;
        
        // Permite usar html en el cuerpo
        mail.IsBodyHtml = true;
        return mail;
    }

    // Enviar correo cuando se crea un incidente
    public async Task SendIncidentCreatedAsync (string toEmail, string toName, string incidentTitle)
    {
        using var smtp = CreateSmtpClient();
        using var mail = CreateBase(toEmail, toName, "Nuevo incidente creado");

        // Contenido de correo en HTML
        mail.Body = $"""
            <h2>Hola {toName},</h2>
            <p>Se ha creado un nuevo incidente en el sistema:</p>
            <p><strong>{incidentTitle}</strong></p>
            <p>Ingresa al panel para ver los detalles.</p>
            <br/>
            <p style="color:#64748b;font-size:12px">IncidentAPI — Notificación automática</p>
        """;

        // Enviar correo de forma asincrona
        await smtp.SendMailAsync(mail);
    }

    // Enviar correo de recuperacion de contraseña
    public async Task SendPasswordResetAsync(string toEmail, string toName, string ResetToken)
    {
        using var smtp = CreateSmtpClient();
        using var mail = CreateBase(toEmail, toName, "Recuperación de contraseña");

        // Contenido del correo con token
        mail.Body = $"""
            <h2>Hola {toName},</h2>
            <p>Recibimos una solicitud para restablecer tu contraseña.</p>
            <p>Tu token de recuperación es:</p>
            <p style="font-size:24px;font-weight:bold;letter-spacing:4px;color:#3b82f6">{ResetToken}</p>
            <p>Este token expira en <strong>15 minutos</strong>.</p>
            <p>Si no solicitaste esto, ignora este correo.</p>
            <br/>
            <p style="color:#64748b;font-size:12px">IncidentAPI — Notificación automática</p>
        """;

        // Enviar correo
        await smtp.SendMailAsync(mail);
    }
}