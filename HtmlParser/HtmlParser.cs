using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlParser
{
    public class HtmlParser
    {
        HtmlDocument Document = new HtmlDocument();
        public void Parse(string html)
        {
            Document.LoadHtml(html);
        }
        public HtmlNode GetForm(string action, string method)
        {
            return Document.DocumentNode.SelectNodes("./form[@action=" + action + " and @method=" + method + "]").FirstOrDefault();
        }
        public HtmlNode GetElementById(string id)
        {
            return Document.GetElementbyId(id);
        }
        public HtmlNodeCollection GetNodes(string xpath)
        {
            return Document.DocumentNode.SelectNodes(xpath);
        }

    }
}
