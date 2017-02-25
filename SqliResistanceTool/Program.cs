using System;
using System.Collections.Generic;
using System.Configuration;
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
            try
            {
                var configSection = (SqliConfig)ConfigurationManager.GetSection("SqliConfig");
                if (configSection == null || configSection.Sites?.Count == 0)
                    return;
                foreach (SiteToProcess item in configSection.Sites)
                {

                    using (var dbContext = new ApplicationDbContext())
                    {

                        var site = dbContext.Sites.FirstOrDefault(m => m.SiteUrlString == item.SiteUri);
                        if (site == null)
                        {
                            site = new SiteModel
                            {
                                SiteUrl = new Uri(item.SiteUri)
                            };
                            if (item.Login != null)
                            {
                                site.LoginInfo = new LoginInfoModel
                                {
                                   
                                    SpecialTextBeforeLoginPage = item.Login.SpecialTextBeforeLoginPage,
                                    SpecialTextAfterLoginPage = item.Login.SpecialTextAfterLoginPage
                                };
                                if (!string.IsNullOrEmpty(item.Login.LoginUri ))
                                   site.LoginInfo.LoginPage = new Uri(site.SiteUrl, item.Login.LoginUri);
                                site.LoginInfo.LoginData = new Dictionary<string, string>();
                                foreach(LoginData rec in item.Login.LoginData)                                
                                    site.LoginInfo.LoginData.Add(rec.Key, rec.Value);                                
                   
                                site.LoginInfo.LoginButton = new ElementSearchModel
                                {
                                    By = item.Login.LoginButton.By,
                                    Value = item.Login.LoginButton.Value
                                };
                            }
                            dbContext.Sites.Add(site);
                            dbContext.SaveChanges();
                        }
                    }
                }
                var crawler = new Crawler();
                crawler.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();
        }
    }
}
