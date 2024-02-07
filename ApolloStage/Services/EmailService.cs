using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly string _emailFrom = "lisboabot@gmail.com";
    private readonly string _password = "wnnz lzgn ydgr lvcj";
    private readonly string _smtpServer = "smtp.gmail.com";

    public async Task SendCodeByEmailAsync(string toEmail, string code)
    {
        try
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient(_smtpServer);

            message.From = new MailAddress(_emailFrom);
            message.To.Add(toEmail);
            message.Subject = "Confirmar registo";
            message.Body = $"O código de verificação para validar a sua conta é: {code}";

            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(_emailFrom, _password);
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao enviar o e-mail: " + ex.Message); //ALTERAR
            throw;
        }
    }

    public async Task SendPasswordByEmailAsync(string toEmail, string code)
    {
        try
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient(_smtpServer);

            message.From = new MailAddress(_emailFrom);
            message.To.Add(toEmail);
            message.Subject = "Redifinir palavra-passe";
            message.Body = $"O código de verificação para alterar a palavra-passe é: {code}";

            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(_emailFrom, _password);
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao enviar o e-mail: " + ex.Message); //ALTERAR
            throw;
        }
    }
}