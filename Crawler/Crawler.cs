using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    public class Crawler
    {
        public string LoginPage { get; set; }
        public string LoginPostUrl { get; set; }
        public string SiteRoot { get; set; }
        public List<string> VisitedPages { get; set; }
        public Queue<string> AvaliablePage { get; set; }
        public Dictionary<string, string> LoginData { get; set; }
        public List<string> Cookies { get; set; }

        Thread thread;
        private bool loginNedded;
        public bool IsRunning { get; private set; }
        public Crawler()
        {
            LoginData = new Dictionary<string, string>();
            VisitedPages = new List<string>();
            AvaliablePage = new Queue<string>();
            Cookies = new List<string>();
            thread = new Thread(new ThreadStart(ThreadWorker));
        }
        public void Start()
        {
            IsRunning = true;
            AvaliablePage.Enqueue(SiteRoot);
            thread.Start();
        }
        public void Stop()
        {
            IsRunning = false;
            thread.Join();

        }
        private void ThreadWorker()
        {
            while (IsRunning)
            {
                try
                {
                    if (AvaliablePage.Count == 0)
                        break;
                    Uri url;
                    string content = null;
                    try
                    {
                        if (loginNedded)
                        {
                            TryToLoggin();
                            continue;
                        }
                        else
                        {
                            var s = AvaliablePage.Dequeue();
                            url = new Uri(s);
                        }
                        var req = CreateRequest(url, content);
                        var res = (HttpWebResponse)req.GetResponse();
                        ProcessResponse(req, res);
                    }
                    catch
                    {
                        continue;
                    }



                }
                catch
                {

                }
            }
            IsRunning = false;
        }
        private HttpWebRequest CreateRequest(Uri page, string content = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(page);
            request.CookieContainer = new CookieContainer();
            foreach (var item in Cookies)
                request.CookieContainer.SetCookies(page, item);
            request.Method = "GET";
            if (content == null)
                return request;
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII))
            {
                writer.Write("content=" + content);
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            return request;
        }
        private void ProcessResponse(HttpWebRequest request, HttpWebResponse response)
        {
            var resUrl = response.ResponseUri.AbsolutePath.ToLower();
            var reqUrl = request.RequestUri.AbsolutePath.ToLower();
            if (resUrl == LoginPage.ToLower())
            {
                loginNedded = true;
                var val = response.Headers["Set-Cookie"];
                Cookies.Add(val);
                return;
            }
            if (response.StatusCode == HttpStatusCode.OK && resUrl == reqUrl)
            {
                ProcessContent(response);
            }
        }
        private void ProcessContent(HttpWebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {

            }
        }
        private void TryToLogin()
        {
            var req = CreateRequest(new Uri(LoginPage));
            var res = req.GetResponse();
            var content="";
            using (var reader = new StreamReader(res.GetResponseStream()))
            {
                content = reader.ReadToEnd();
            }
            HtmlParser.HtmlParser parser = new HtmlParser.HtmlParser();
            parser.Parse(content);
            parser.GetForm()
        }
    }
}
