using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeleniumCrawler;
using SqliResistanceModel;

namespace SqliResistanceTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = new Uri("http://localhost:9797/dvwa/");
            using (var dbContext = new ApplicationDbContext())
            {

                var site = dbContext.Sites.FirstOrDefault(m => m.SiteUrlString == url.AbsoluteUri);
                if (site == null)
                {
                    site = new SiteModel
                    {
                        SiteUrl = url
                    };
                    site.LoginInfo = new LoginInfoModel
                    {
                        LoginPage = new Uri(site.SiteUrl, "login.php")
                    };
                    site.LoginInfo.LoginData = new Dictionary<string, string>
                    {
                        {"username", "admin"},
                        {"password", "password"}
                    };
                    site.LoginInfo.LoginButton = new ElementSearchModel
                    {
                        By = SearchBy.Name,
                        Value = "Login"
                    };
                    dbContext.Sites.Add(site);
                    dbContext.SaveChanges();
                }
                else
                {
                    site.VisitedLinks.Clear();
                    site.AvailableLinks.Clear();
                    site.CrawlingDone = false;
                    dbContext.Entry(site).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }


            }
            var crawler = new Crawler();
            crawler.Start();
        }
    }
}
