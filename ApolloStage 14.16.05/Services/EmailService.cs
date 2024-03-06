using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly string _emailFrom = "lisboabot@gmail.com"; // Substitua pelo seu e-mail remetente
    private readonly string _password = "wnnz lzgn ydgr lvcj"; // Substitua pela senha do seu e-mail remetente
    private readonly string _smtpServer = "smtp.gmail.com"; // Substitua pelo seu servidor SMTP

    public async Task SendCodeByEmailAsync(string toEmail, string code)
    {
        try
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient(_smtpServer);

            message.From = new MailAddress(_emailFrom);
            message.To.Add(toEmail);
            message.Subject = "Seu Código de Verificação";
            message.Body = $"Seu código de verificação é: {code}";

            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(_emailFrom, _password);
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Lida com possíveis erros no envio do e-mail
            Console.WriteLine("Erro ao enviar o e-mail: " + ex.Message);
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
            message.Subject = "New Password";
            message.Body = $"A sua nova password é: {code}";

            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(_emailFrom, _password);
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Lida com possíveis erros no envio do e-mail
            Console.WriteLine("Erro ao enviar o e-mail: " + ex.Message);
            throw;
        }
    }
}
