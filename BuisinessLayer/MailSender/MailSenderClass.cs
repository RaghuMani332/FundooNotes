using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.MailSender
{
    public static class MailSenderClass
    {
        public static void sendMail(String ToMail, String otp)
        {
            /*SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            NetworkCredential credential = new NetworkCredential("subamraghu11154@gmail.com","Raghu2k02");
           
            client.Credentials = credential;
            MailMessage message = new MailMessage("c", ToMail);
            message.Subject = $"To Reset Your Password Enter the Below OTP \n {otp}";
            message.Body = "<h1> this is a mail regarding password change</h1>";
            message.IsBodyHtml = true;
            client.Send(message);*/
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("subamraghu11154@gmail.com", "Raghu2k02");

            MailMessage message = new MailMessage();
            message.From = new MailAddress("subamraghu11154@gmail.com");
            message.To.Add(new MailAddress(ToMail));
            message.Subject = "Password Reset OTP";
            message.Body = $"To reset your password, please use the following OTP: {otp}";

            try
            {
                client.Send(message);
                Console.WriteLine("Mail sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending mail daa: " + ex.StackTrace);
            }
        }
    }
    }
