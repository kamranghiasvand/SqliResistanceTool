using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;

namespace SqliResistanceModel
{
    public class SiteModel
    {
        [Key]
        public int Id { get; set; }
        [DataType(DataType.Url)]
        public string SiteUrlString { get; set; }

        private Uri siteurl;
        [NotMapped]
        public Uri SiteUrl
        {
            get { return siteurl ?? (siteurl = new Uri(SiteUrlString)); }
            set
            {
                siteurl = value;
                SiteUrlString = siteurl.AbsoluteUri;
            }
        }

        public virtual ICollection<PageModel> Pages { get; set; } = new List<PageModel>();
        public virtual LoginInfoModel LoginInfo { get; set; }
        public virtual ICollection<string> VisitedLinks { get; set; } = new List<string>();
        public virtual ICollection<string> AvailableLinks { get; set; } = new List<string>();
        public bool CrawlingDone { get; set; }
        public DateTime StartDate { get; set; } = new DateTime(2000, 1, 1);
        public DateTime FinishDate { get; set; } = new DateTime(2000, 1, 1);
        public string LastFailReason { get; set; }
        public bool IsExternalLink(Uri link)
        {
            var another = link.Scheme + "://" + link.Authority;
            var self = SiteUrl.Scheme + "://" + SiteUrl.Authority;
            return !(new Uri(self).Equals(new Uri(another)));

        }
    }
    public class LoginInfoModel
    {
        [Key]
        public int Id { get; set; }
        [DataType(DataType.Url)]
        public string LoginPageString { get; set; }

        Uri loginPage;

        [NotMapped]
        public Uri LoginPage
        {
            get { return loginPage ?? (loginPage = new Uri(LoginPageString)); }
            set
            {
                loginPage = value;
                LoginPageString = loginPage.AbsoluteUri;
            }
        }
        [NotMapped]
        public Dictionary<string, string> LoginData { get; set; }
        public string LoginDataAsXml
        {
            get { return SerializeDict(LoginData); }
            set { LoginData = DeserializeDict(value); }
        }
        public virtual ElementSearchModel LoginButton { get; set; }

        public bool IsValid()
        {
            return LoginPage != null && LoginData.Count > 1 && LoginButton != null;
        }

        public static string SerializeDict(IDictionary<string, string> dict)
        {
            var el = new XElement("root",
                dict.Select(kv => new XElement(kv.Key, kv.Value)));
            return el.ToString();
            //// serialize the dictionary
            //var serializer = new DataContractSerializer(dict.GetType());
            //using (var sw = new StringWriter())
            //{
            //    using (var writer = new XmlTextWriter(sw))
            //    {
            //        // add formatting so the XML is easy to read in the log
            //        writer.Formatting = (Formatting)Formatting.Indented;

            //        serializer.WriteObject(writer, dict);

            //        writer.Flush();

            //        return sw.ToString();
            //    }
            //}
        }

        public static Dictionary<string, string> DeserializeDict(string dic)
        {
            var rootElement = XElement.Parse(dic);
            return rootElement.Elements().ToDictionary(el => el.Name.LocalName, el => el.Value);
        }


    }
}
