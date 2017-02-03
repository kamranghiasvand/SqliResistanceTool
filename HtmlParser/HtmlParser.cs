using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

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
            try
            {
                if (action == null)
                    action = "";
                if (method == null)
                    method = "";
                action = action.ToLower();
                method = method.ToLower();
                //XPathNavigator nav = Document.CreateNavigator();

                //// Create the custom context and add the namespace to the context
                //XsltCustomContext ctx = new XsltCustomContext(new NameTable());
                //ctx.AddNamespace("Extensions", XsltCustomContext.NamespaceUri);

                //// Build the XPath query using the new function
                //XPathExpression xpath =
                //  XPathExpression.Compile("//form[@action[Extensions:CaseInsensitiveComparison('" + action + "')] and @method[Extensions:CaseInsensitiveComparison('" + method + "')]");

                //// Set the context for the XPath expression to the custom context containing the 
                //// extensions
                //xpath.SetContext(ctx);

                //var element = nav.SelectSingleNode(xpath);              

                return Document.DocumentNode.SelectSingleNode("//form[translate(@action,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" + action + "' "+
                "and translate(@method,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" + method + "']");
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public HtmlNodeCollection GetForms(Dictionary<string,string> attribs)
        {          
            if (attribs == null)
                throw new ArgumentNullException("attrib", "");
            try
            {
                if (attribs.Count == 0)
                    return Document.DocumentNode.SelectNodes("//form");
                var restrict = "";
                foreach (var key in attribs.Keys)
                {
                    restrict += "translate(@" + key + ",'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" + attribs[key].ToLower() + "'";
                    restrict += " and ";
                }
                restrict = restrict.Substring(0, restrict.Length - 5);
                return Document.DocumentNode.SelectNodes("//form[" + restrict + "]");
            }
            catch(Exception ex)
            {
                return null;
            }
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
