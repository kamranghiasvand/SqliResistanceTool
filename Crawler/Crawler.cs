using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace Crawler
{
    public class Crawler
    {
        public Uri LoginPage { get; set; }

        public Uri SiteRoot { get; set; }
        public List<Uri> VisitedPages { get; set; }
        public Queue<Uri> AvaliablePage { get; set; }
        public Dictionary<string, string> LoginData { get; set; }
        public List<string> Cookies { get; set; }

        private readonly Thread thread;
        private bool loginNedded;
        private WebHeaderCollection header = new WebHeaderCollection();
        public bool IsRunning { get; private set; }
        public Crawler()
        {
            LoginData = new Dictionary<string, string>();
            VisitedPages = new List<Uri>();
            AvaliablePage = new Queue<Uri>();
            Cookies = new List<string>();
            thread = new Thread(ThreadWorker);
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
                    try
                    {
                        if (loginNedded)
                        {
                            if (!TryToLogin())
                                return;
                            continue;
                        }
                        if (AvaliablePage.Count == 0)
                            break;
                        var s = AvaliablePage.Dequeue();
                        var url = new Uri(SiteRoot, s);
                        var req = CreateRequest(url, header, null, null);
                        var res = (HttpWebResponse)req.GetResponse();
                        var isLoggined = false;
                        ProcessResponse(req, res, ref isLoggined);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                catch
                {
                    // ignored
                }
            }
            IsRunning = false;
        }
        private HttpWebRequest CreateRequest(Uri page, WebHeaderCollection header, string contentType, string content, bool KeppAlive = true)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(SiteRoot, page));
            //if (header != null)
            //    foreach (string key in header.Keys)
            //    {
            //        if (key.ToLower() == "date" || key.ToLower() == "set-cookie" || key.ToLower() == "content-length" || key.ToLower() == "content-type")
            //            continue;
            //        request.Headers.Add(key, header[key]);
            //    }
            request.Date = DateTime.Now;
            request.CookieContainer = new CookieContainer();
            foreach (var item in Cookies)
                request.CookieContainer.SetCookies(page, item);
            request.Method = "GET";
            if (content == null)
                return request;
            request.ContentType = contentType;
            request.Method = "POST";
            // request.ContentLength = Encoding.UTF8.GetByteCount(content);
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                //writer.Write(Encoding.UTF8.GetBytes(content));
                writer.Write(content);
            }

            return request;
        }
        private void ProcessResponse(HttpWebRequest request, HttpWebResponse response, ref bool isLoggined)
        {
            var resUrl = response.ResponseUri;
            var reqUrl = request.RequestUri;

            #region From login page To another page
            if (reqUrl.Equals(LoginPage) && !resUrl.Equals(LoginPage))
            {
                isLoggined = true;
                loginNedded = false;
                var val = response.Headers["Set-Cookie"];
                if (val != null)
                    Cookies.Add(val);
                return;
            }
            #endregion




            if (!reqUrl.Equals(resUrl) && resUrl.Equals(LoginPage))
            {
                loginNedded = true;
                var val = response.Headers["Set-Cookie"];
                if (val != null)
                    Cookies.Add(val);
                return;
            }
            if (response.StatusCode == HttpStatusCode.OK && resUrl.Equals(reqUrl))
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
        private bool TryToLogin()
        {
            var req = CreateRequest(LoginPage, header, null, null);
            var res = (HttpWebResponse)req.GetResponse();
            header = res.Headers;
            var content = "";
            using (var reader = new StreamReader(res.GetResponseStream()))
            {
                content = reader.ReadToEnd();
            }
            var parser = new HtmlParser.Parser();
            parser.Load(content);

            var loginform = parser.GetForm("login.php", "POST");
            if (loginform == null)
                loginform = parser.GetForm(LoginPage.AbsolutePath.Substring(1), "POST");
            if (loginform == null)
                return false;
            content = "";
            foreach (var node in loginform.SelectNodes("//input"))
            {
                var name = node.Attributes["name"];
                var value = node.Attributes["value"];
                if (string.IsNullOrEmpty(name?.Value))
                    continue;
                if (LoginData.Keys.Contains(name.Value))
                {
                    content += name.Value + "=" + LoginData[name.Value] + "&";
                }
                else if (value != null && !string.IsNullOrEmpty(value.Value))
                {
                    content += name.Value + "=" + value.Value + "&";
                }
                else
                {
                    content += name.Value + "=&";
                }
            }
            header["User-Agent"] =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
            header["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            header["Referer"] = "http://localhost:9797/dvwa/login.php";
            header["Accept-Encoding"] = "gzip, deflate, br";
            header["Accept-Language"] = "en-US,en;q=0.8,fa;q=0.6";
            req = CreateRequest(LoginPage, header, "application/x-www-form-urlencoded;charset=utf-8", content.Substring(0, content.Length - 1), false);
            res = (HttpWebResponse)req.GetResponse();
            if (!string.IsNullOrEmpty(res.Headers["Set-Cookie"]))
                Cookies.Add(res.Headers["Set-Cookie"]);

            var isLoggined = false;
            var text = new StreamReader(res.GetResponseStream()).ReadToEnd();
            ProcessResponse(req, res, ref isLoggined);
            return isLoggined;
        }


    }
}
