using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Jokes
{
    class Program
    {

        static void Main(string[] args)
        {
            SendMessage.Main1(args);
        }


    }
    class SendMessage
    {
        private const string URL = "https://disify.com/";

        static HttpClient client = new HttpClient();

        static HttpClient dadJoke = new HttpClient();
        public static void Main1(string[] args)
        {

            dadJoke.BaseAddress = new Uri("https://icanhazdadjoke.com/");
            string joke = jetJoke().GetAwaiter().GetResult();

            Console.WriteLine("Please enter email");
            string addressTo = Console.ReadLine();
            Console.WriteLine("Please enter password");
            string pass = Console.ReadLine();

            if (ValidateAdress(addressTo).GetAwaiter().GetResult())
            {
                var result = SendJoke(joke, addressTo, pass).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Email adress is not valid");
            }
        }

        static async Task<string> jetJoke()
        {
            HttpResponseMessage response = await dadJoke.GetAsync("/slack");
            string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            object o = new JavaScriptSerializer().Deserialize<object>(result);
            string joke = ((Dictionary<string, object>)(((object[])((Dictionary<string, object>)o)["attachments"])[0]))["text"] as string;

            return joke;
        }

        static async Task<bool> SendJoke(string joke, string addressTo, string pass)
        {
            string body;
            body = "<html dir='ltr' lang='en'>";
            body += "<head>";
            body += "<meta charset='UTF-8'>";
            body += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
            body += "</head>";
            body += "<body dir='ltr' lang='en'>";
            body += joke;
            body += "</body>";
            body += "</html>";

            

            SmtpClient smtpClient = new SmtpClient("glassix-hmail.westeurope.cloudapp.azure.com");
            smtpClient.UseDefaultCredentials = false;

            NetworkCredential credential = new NetworkCredential("test@glassix-spam.com", pass);
            smtpClient.Credentials = credential;


            MailAddress addressFrom = new MailAddress("test@glassix-spam.com");
            MailMessage message = new MailMessage();
            message.From = addressFrom;
            message.To.Add(addressTo);
            message.Subject = "TestJoke";
            message.IsBodyHtml = true;
            message.Body = body;

            try
            {
                smtpClient.Send(message);

            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.InnerException.Message);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }

            return true;
        }

        static async Task<bool> ValidateAdress(string adress)
        {
            client.BaseAddress = new Uri(URL);
            HttpResponseMessage response = await client.GetAsync("api/email/" + adress);
            string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Dictionary<string, object> o = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
            bool isAdressValid = (bool)o["format"];
            return isAdressValid;
        }
    }
}
