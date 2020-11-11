using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private const string URL = "https://api.eu.mailgun.net/v3/mg.brash.io/messages";

        static HttpClient client = new HttpClient();

        static HttpClient dadJoke = new HttpClient();
        public static void Main1(string[] args)
        {

            client.BaseAddress = new Uri(URL);
            dadJoke.BaseAddress = new Uri("https://icanhazdadjoke.com/");
            string joke = jetJoke().GetAwaiter().GetResult();

            var result = SendJoke(joke).GetAwaiter().GetResult();
        }

        static async Task<string> jetJoke()
        {
            HttpResponseMessage response = await dadJoke.GetAsync("/slack");
            string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            object o = new JavaScriptSerializer().Deserialize<object>(result);
            string joke = ((Dictionary<string, object>)(((object[])((Dictionary<string, object>)o)["attachments"])[0]))["text"] as string;

            return joke;
        }

        static async Task<bool> SendJoke(string joke)
        {
           // client.DefaultRequestHeaders.Authorization = 
           //     new AuthenticationHeaderValue("Basic", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes("api" + ":" + "key-xxxxxxxxxxx")));

            bool t = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=xxxxxxx");

            var form = new Dictionary<string, string>();

            form["from"] = "<olyaalchemy@gmail.com>";
            form["to"] = "<olyaalchemy@gmail.com>";
            form["subject"] = "Dad joke test";

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

            form["html"] = Regex.Replace(body, @"\r\n?|\n", "<br />");

            var response = await client.PostAsync("https://api.mailgun.net/v2/" + "reach.softnames.com" + "/messages", new FormUrlEncodedContent(form));

            bool b = response.IsSuccessStatusCode;

            return true;
        }
    }
}
