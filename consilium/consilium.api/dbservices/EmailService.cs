using EmailAuthenticator;
using MimeKit;

namespace Consilium.API.DBServices;

public class EmailService(IConfiguration config) : IEmailService {
    public async Task SendValidationEmail(string email, string validationToken) {
        string link = $"https://consilium-api-cpgdcqaxepbyc2gj.westus3-01.azurewebsites.net/validate?email={email}&token={validationToken}";
        Console.WriteLine(link);

        await SendEmail(email, link);
    }

    private async Task SendEmail(string email, string link) {

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(config["EmailSettings:SenderName"], config["EmailSettings:SenderEmail"]));

        string username = email.Split('@')[0];
        emailMessage.To.Add(new MailboxAddress(username, email));
        emailMessage.Subject = "Confirm your email";

        emailMessage.Body = new TextPart("html") {
            Text = $"""
                <div style="background-color:#FAF7F2; padding: 40px; font-family: 'Inter', sans-serif; color: #242424;">
                    <div style="max-width: 600px; margin: auto; background-color: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 10px rgba(0,0,0,0.05);">
                        <h1 style="color: #2E4237; font-size: 28px; margin-top: 0;">Welcome to Consilium!</h1>
                        <p style="font-size: 16px; line-height: 1.6;">
                            An account token has been associated with this email.
                            Please confirm your account by clicking the button below:
                        </p>
                        <br/>
                        <div style="text-align: center; margin: 30px 0;">
                            <a href="{link}" style="background-color: #2E4237; color: white; padding: 12px 24px; text-decoration: none; border-radius: 8px; font-weight: 600;">
                                Confirm Account
                            </a>
                        </div>
                        <br/>
                        <p style="font-size: 14px; color: #54695D;">
                            If this wasn't you, no action is required. You can safely ignore this email.
                        </p>
                        <hr style="margin: 40px 0; border: none; border-top: 1px solid #E3D6BF;" />
                        <p style="font-size: 14px; text-align: center; color: #54695D;">
                            Thanks,<br/>
                            The Consilium Team
                        </p>
                    </div>
                </div>
                """
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();
        if (config["EmailSettings:Port"] is null) {
            throw new Exception("Port cannot be null");
        }
        int port = int.Parse(config["EmailSettings:Port"]!);
        await client.ConnectAsync(config["EmailSettings:SmtpServer"], port, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(config["EmailSettings:Username"], config["EmailSettings:Password"]);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}