﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using HtmlAgilityPack;
using HtmlParser;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SqliResistanceModel;

namespace SeleniumCrawler
{
    public class Crawler
    {

        public bool IsRunning { get; private set; }

        private readonly IWebDriver driver = new FirefoxDriver();
        private readonly Thread thread;

        public Crawler()
        {
            thread = new Thread(Worker);
        }
        public void Start()
        {
            IsRunning = true;
            thread.Start();

        }

        public void Stop()
        {
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
                            break;
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
                            if (site.VisitedLinks.Contains(url.AbsoluteUri))
                                continue;

                            site.VisitedLinks.Add(url.AbsoluteUri);
                            dbContext.Entry(site).State = EntityState.Modified;
                            dbContext.SaveChanges();

                            driver.Navigate().GoToUrl(url);
                            if (site.LoginInfo != null && site.LoginInfo.IsValid() && site.LoginInfo.LoginPage.Equals(new Uri(driver.Url)))
                                if (!TryLogin(site))
                                {
                                    failedToLogin = true;
                                    break;
                                }
                            ParseContent(site, dbContext, url);
                        }
                        if (failedToLogin)
                        {
                            site.LastFailReason = "Failed To Login";
                            site.FinishDate = DateTime.Now;
                            site.CrawlingDone = true;
                            dbContext.Entry(site).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }

                }
                catch (Exception)
                {
                    // ignored
                }
            }
            driver.Quit();
        }

        bool TryLogin(SiteModel site)
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
                return !site.LoginInfo.LoginPage.Equals(new Uri(driver.Url));
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ParseContent(SiteModel site, ApplicationDbContext dbContext, Uri url)
        {
            try
            {
                #region Parse Content
                var content = driver.PageSource;
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
                    foreach (var input in item.SelectNodes("//input"))
                    {
                        var finput = HtmlNodeInputToFormInputModel(input);
                        if (finput != null)
                            form.Inputs.Add(finput);
                    }
                    page.Forms.Add(form);
                    dbContext.Entry(page).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }
                #endregion

                #region Links

                foreach (var link in  parser.GetLinks())
                {
                    var attr = link.Attributes["href"];
                    if (string.IsNullOrEmpty( attr?.Value))
                        continue;
                    var href = new Uri(url, attr.Value);
                    if (site.VisitedLinks.Contains(href.AbsoluteUri))
                        continue;
                    if (site.IsExternalLink(href))
                        continue;
                    site.AvailableLinks.Add(href.AbsoluteUri);
                }

                #endregion

                #endregion
            }
            catch (Exception ex)
            {

                throw;
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
                    finput.Checked = bool.Parse(attr.Value);
                attr = input.Attributes["disabled"];
                if (attr != null)
                    finput.Disabled = bool.Parse(attr.Value);
                attr = input.Attributes["maxlength"];
                if (attr != null)
                    finput.Maxlength = int.Parse(attr.Value);
                attr = input.Attributes["name"];
                if (attr != null)
                    finput.Name = attr.Value;
                attr = input.Attributes["readonly"];
                if (attr != null)
                    finput.Readonly = bool.Parse(attr.Value);
                attr = input.Attributes["size"];
                if (attr != null)
                    finput.Size = int.Parse(attr.Value);
                attr = input.Attributes["src"];
                if (attr != null)
                    finput.Src = attr.Value;
                attr = input.Attributes["type"];
                if (attr != null)
                    finput.Type = (InputType)Enum.Parse(typeof(InputType), attr.Value);
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
