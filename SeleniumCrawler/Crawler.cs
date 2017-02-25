using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlParser;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SqliResistanceModel;

namespace SeleniumCrawler
{
    public class Crawler
    {

        public bool IsRunning { get; private set; }
        string script = "window.__mySecretGlobal = 'false'; " +
             "window.addEventListener('load', function(){ " +
                  "window.__mySecretGlobal = 'true'; " +
             "});";

        private IWebDriver driver;
        private readonly Thread thread;

        public Crawler()
        {
            thread = new Thread(Worker);
        }
        public void Start()
        {
            if (IsRunning)
                return;
            IsRunning = true;
           
            thread.Start();

        }

        public void Stop()
        {
            if (!IsRunning)
                return;
            IsRunning = false;
            thread?.Join();
        }

        private void Worker()
        {

            while (IsRunning)
            {
                try
                {
                    using (var dbContext = new ApplicationDbContext())
                    {
                        var site = dbContext.Sites.FirstOrDefault(m => !m.CrawlingDone);
                        if (site == null)
                        {
                            IsRunning = false;
                            break;
                        }
                        if (site.StartDate.Equals(new DateTime(2000, 1, 1)))
                        {
                            site.StartDate = DateTime.Now;
                            dbContext.Entry(site).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        if (site.AvailableLinks.Count == 0)
                            site.AvailableLinks.Add(site.SiteUrl.AbsoluteUri);
                        var failedToLogin = false;
                        while (site.AvailableLinks.Count > 0)
                        {
                            var urlstring = site.AvailableLinks.First();
                            site.AvailableLinks.Remove(urlstring);
                            var url = new Uri(site.SiteUrl, urlstring);
                            if (site.Pages.Any(m => m.Url.Equals(url)))
                                continue;
                            if(driver==null)
                                driver = new ChromeDriver();
                            
                            driver.Navigate().GoToUrl(url);
                            Thread.Sleep(300);                          
                            if (site.LoginInfo != null && site.LoginInfo.IsValid() && site.LoginInfo.LoginPage!=null&& site.LoginInfo.LoginPage.Equals(new Uri(driver.Url)))
                                if (!TryLoginWithUrl(site))
                                {
                                    failedToLogin = true;
                                    break;
                                }
                                else
                                    driver.Navigate().GoToUrl(url);

                           ParseContent(site, dbContext, url,out failedToLogin);
                        }
                        if (failedToLogin)
                        {
                            site.LastFailReason = "Failed To Login";
                            site.FinishDate = DateTime.Now;
                            site.CrawlingDone = true;
                            dbContext.Entry(site).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        site.FinishDate = DateTime.Now;
                        site.CrawlingDone = true;
                        dbContext.Entry(site).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                }
                catch (Exception)
                {
                    // ignored
                }
            }
            driver?.Close();
        }
    
        private bool IsInList(ICollection<string> list, Uri link)
        {

            try
            {
                return list != null && list.Select(item => new Uri(item)).Contains(link);
            }
            catch
            {
                return true;

            }
        }
        bool TryLoginWithUrl(SiteModel site)
        {
            try
            {
                foreach (var key in site.LoginInfo.LoginData.Keys)
                    driver.FindElement(By.Name(key)).SendKeys(site.LoginInfo.LoginData[key]);
                IWebElement element = null;
                switch (site.LoginInfo.LoginButton.By)
                {
                    case SqliResistanceModel.SearchBy.ClassName:
                        element = driver.FindElement(By.ClassName(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.CssSelector:
                        element = driver.FindElement(By.CssSelector(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.Id:
                        element = driver.FindElement(By.Id(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.LinkText:
                        element = driver.FindElement(By.LinkText(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.Name:
                        element = driver.FindElement(By.Name(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.PartialLinkText:
                        element = driver.FindElement(By.PartialLinkText(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.TagName:
                        element = driver.FindElement(By.TagName(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.XPath:
                        element = driver.FindElement(By.XPath(site.LoginInfo.LoginButton.Value));
                        break;
                }
                element?.Click();
                Thread.Sleep(2000);
                return !site.LoginInfo.LoginPage.Equals(new Uri(driver.Url));
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        bool TryLoginWithContent(SiteModel site)
        {
            try
            {
                if (string.IsNullOrEmpty(site.LoginInfo.SpecialTextAfterLoginPage))
                    return false;
                foreach (var key in site.LoginInfo.LoginData.Keys)
                    driver.FindElement(By.Name(key)).SendKeys(site.LoginInfo.LoginData[key]);
                IWebElement element = null;
                switch (site.LoginInfo.LoginButton.By)
                {
                    case SqliResistanceModel.SearchBy.ClassName:
                        element = driver.FindElement(By.ClassName(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.CssSelector:
                        element = driver.FindElement(By.CssSelector(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.Id:
                        element = driver.FindElement(By.Id(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.LinkText:
                        element = driver.FindElement(By.LinkText(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.Name:
                        element = driver.FindElement(By.Name(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.PartialLinkText:
                        element = driver.FindElement(By.PartialLinkText(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.TagName:
                        element = driver.FindElement(By.TagName(site.LoginInfo.LoginButton.Value));
                        break;
                    case SqliResistanceModel.SearchBy.XPath:
                        element = driver.FindElement(By.XPath(site.LoginInfo.LoginButton.Value));
                        break;
                }
                element?.Click();
                Thread.Sleep(2000);
                if (string.IsNullOrEmpty(driver.PageSource))
                    return false;
               
                if(driver.PageSource.Contains(site.LoginInfo.SpecialTextAfterLoginPage))
                {
                    site.AvailableLinks.Add(driver.Url);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool IsLoginPageContent(SiteModel site, string content)
        {
            if (string.IsNullOrEmpty(site.LoginInfo.SpecialTextBeforeLoginPage))
                return false;
            if (string.IsNullOrEmpty(content))
                return false;
            return content.Contains(site.LoginInfo.SpecialTextBeforeLoginPage);
        }
        private void ParseContent(SiteModel site, ApplicationDbContext dbContext, Uri url,out bool failedToLogin)
        {
            failedToLogin = false;
            try
            {
                var content = driver.PageSource;
                if (IsLoginPageContent(site,content)&& !TryLoginWithContent(site))
                {
                    failedToLogin=true;
                }
                #region Parse Content
                var page = new PageModel
                {
                    Url = url,
                    VisitedDate = DateTime.Now,
                    RawContent = content
                };
                site.Pages.Add(page);
                dbContext.Entry(site).State = EntityState.Modified;
                dbContext.SaveChanges();
                var parser = new Parser();
                parser.Load(content);

                #region Forms
                var forms = parser.GetForms();

                foreach (var item in forms)
                {
                    var form = HtmlNodeFormToFormModel(item);
                    if (form == null)
                        continue;
                    form.Page = page;
                    try
                    {
                        foreach (var input in item.SelectNodes(".//input"))
                        {
                            try
                            {
                                var finput = HtmlNodeInputToFormInputModel(input);
                                if (finput != null)
                                    form.Inputs.Add(finput);
                            }
                            catch { }
                        }
                    }
                    catch { }
                    try
                    {
                        foreach (var select in item.SelectNodes(".//select"))
                        {
                            try
                            {
                                var fselect = HtmlNodeSelectToFormSelectModel(select);
                                if (fselect != null)
                                    form.Selects.Add(fselect);
                            }
                            catch { }
                        }
                    }
                    catch { }

                    page.Forms.Add(form);
                    dbContext.Entry(page).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
                #endregion

                #region Links

                foreach (var link in parser.GetLinks())
                {
                    try
                    {

                        var attr = link.Attributes["href"];
                        if (string.IsNullOrEmpty(attr?.Value))
                            continue;
                        var href = new Uri(url, attr.Value);
                        if (site.Pages.Any(m => m.Url.Equals(href)))
                            continue;
                        if (site.IsExternalLink(href))
                            continue;
                        if (!IsInList(site.AvailableLinks, href))
                            site.AvailableLinks.Add(href.AbsoluteUri);
                    }
                    catch
                    {

                    }
                }

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
            }
        }
        private FormModel HtmlNodeFormToFormModel(HtmlNode item)
        {
            try
            {
                var form = new FormModel();
                var attr = item.Attributes["accept-charset"];
                if (attr != null)
                    form.AcceptCharset = attr.Value;
                attr = item.Attributes["action"];
                if (attr != null)
                    form.Action = attr.Value;
                attr = item.Attributes["enctype"];
                if (attr != null)
                    form.Enctype = attr.Value;
                attr = item.Attributes["method"];
                if (attr != null)
                    form.Method = attr.Value;
                attr = item.Attributes["name"];
                if (attr != null)
                    form.Name = attr.Value;
                attr = item.Attributes["target"];
                if (attr != null)
                    form.Target = attr.Value;
                return form;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private FormSelectModel HtmlNodeSelectToFormSelectModel(HtmlNode select)
        {
            try
            {
                var fselect = new FormSelectModel();
                var attr = select.Attributes["name"];
                if (attr != null)
                    fselect.Name = attr.Value;
                attr = select.Attributes["disabled"];
                if (attr != null)
                {
                    if (attr.Value?.ToLower() == "disabled")
                        fselect.Disabled = true;
                    var res = false;
                    if (bool.TryParse(attr.Value, out res))
                        fselect.Disabled = res;
                }
                foreach (var op in select.SelectNodes(".//option"))
                {
                    attr = op.Attributes["value"];
                    if (attr != null)
                        fselect.Values.Add(attr.Value);
                }
                return fselect;
            }
            catch
            {
                return null;
            }
        }
        private FormInputModel HtmlNodeInputToFormInputModel(HtmlNode input)
        {
            try
            {
                var finput = new FormInputModel();
                var attr = input.Attributes["alt"];
                if (attr != null)
                    finput.Alt = attr.Value;
                attr = input.Attributes["checked"];
                if (attr != null)
                {
                    if (attr.Value?.ToLower() == "checked")
                        finput.Checked = true;
                    var res = false;
                    if (bool.TryParse(attr.Value, out res))
                        finput.Checked = res;
                }
                attr = input.Attributes["disabled"];
                if (attr != null)
                {
                    if (attr.Value?.ToLower() == "disabled")
                        finput.Disabled = true;
                    var res = false;
                    if (bool.TryParse(attr.Value, out res))
                        finput.Disabled = res;
                }
                attr = input.Attributes["maxlength"];
                if (attr != null)
                    finput.Maxlength = int.Parse(attr.Value);
                attr = input.Attributes["name"];
                if (attr != null)
                    finput.Name = attr.Value;
                attr = input.Attributes["readonly"];
                if (attr != null)
                {
                    var res = false;
                    if (bool.TryParse(attr.Value, out res))
                        finput.Readonly = res;
                   
                }
                attr = input.Attributes["size"];
                if (attr != null)
                {
                    int res = 0;
                    if (int.TryParse(attr.Value, out res))
                        finput.Size = res;
                }
                attr = input.Attributes["src"];
                if (attr != null)
                    finput.Src = attr.Value;
                attr = input.Attributes["type"];
                if (attr != null)
                {
                    InputType type = InputType.Text;
                    if (Enum.TryParse(attr.Value, true, out type))
                        finput.Type = type;
                }
                attr = input.Attributes["value"];
                if (attr != null)
                    finput.Value = attr.Value;
                return finput;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
