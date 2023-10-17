using FileStorage.EmailMessaging.Models.Incoming;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace FileStorage.EmailMessaging.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MessagesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> PostMessage(MessageRegisterDto data)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_configuration.GetSection("EmailConfig:Name").Value!, _configuration.GetSection("EmailConfig:Login").Value!));
            email.To.Add(MailboxAddress.Parse(data.Email));
            email.Subject = "Registration";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = @"
              <div style=""font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif;width:100%!important;height:100%!important;"">
                <table style=""box-sizing:border-box;border-collapse:separate!important;width:100%;background-color:#fff"" width=""100%"" bgcolor=""#fff"">
                  <tbody><tr>
                    <td style=""box-sizing:border-box;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Helvetica,Arial,sans-serif,'Apple Color Emoji','Segoe UI Emoji','Segoe UI Symbol';font-size:14px;vertical-align:top"" valign=""top""></td>
                    <td style=""box-sizing:border-box;text-align:center;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Helvetica,Arial,sans-serif,'Apple Color Emoji','Segoe UI Emoji','Segoe UI Symbol';font-size:14px;vertical-align:top;display:block;max-width:580px;width:580px;margin:0 auto;padding:24px"" width=""580"" valign=""top"">
                      <div style=""max-width:640px;border-radius:16px;padding:0px 16px;padding-bottom:8px;transform:translate(-50% 0%);left:50%;"">
                        <p style=""margin:32px 0px 12px 0px;font-size: 24px;font-weight:700;"">Hello Kitty!</p>
                        <p style=""font-size:20px;"">Thanks for signing up.</p>
                        <p style=""font-size:20px;"">Please confirm your email below.</p>
                        <a href=""" + data.Url + @""" style=""background-color: rgb(255, 50, 50);padding:12px 16px;color:white;border:none;transition:all .25s ease;margin:32px 0px;
                        margin-bottom: 24px;outline:none;border-radius:4px;font-weight:600;font-size:18px;cursor:pointer;text-decoration:none;"">Verify Email</a>
                        <p style=""text-wrap:balance;font-size:18px;margin-bottom:4px;"">Or copy and paste link into your browser:</p>
                        <div class=""border:1px solid #ccc;width:100%;padding:4px;font-weight:400;text-wrap:balance;
                        grid-template-columns:1fr 30px;display:grid;border-radius:8px;align-items:center;"">
                          <span class=""vertical-align:middle;text-align:left;color:#222;max-width:100vw;"">" + data.Url + @"</span>
                        </div>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </div>"
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailConfig:Smtp").Value!, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailConfig:Login").Value!, _configuration.GetSection("EmailConfig:Password").Value!);
            smtp.Send(email);
            smtp.Disconnect(true);

            return Ok();
        }
    }
}
