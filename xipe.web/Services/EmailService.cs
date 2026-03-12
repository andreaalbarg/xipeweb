using System.Net;
using System.Net.Mail;

namespace xipe.web.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendContactFormAsync(string name, string email, string company, string service, string message)
    {
        var settings = _config.GetSection("EmailSettings");

        var smtp = new SmtpClient(settings["SmtpHost"])
        {
            Port = int.Parse(settings["SmtpPort"]!),
            Credentials = new NetworkCredential(settings["SmtpUser"], settings["SmtpPassword"]),
            EnableSsl = true
        };

        var body = $"""
            Nuevo contacto desde la página web de Xipe

            Nombre:   {name}
            Email:    {email}
            Empresa:  {company}
            Servicio: {service}

            Mensaje:
            {message}
            """;

        var mail = new MailMessage
        {
            From = new MailAddress(settings["FromAddress"]!, "Xipe Website"),
            Subject = $"Nuevo contacto: {name} – {company}",
            Body = body,
            IsBodyHtml = false
        };

        mail.To.Add(settings["ToAddress"]!);
        mail.ReplyToList.Add(new MailAddress(email, name));

        await smtp.SendMailAsync(mail);
    }
}
