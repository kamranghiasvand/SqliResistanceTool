using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace HtmlParser
{
    public class Parser
    {
        readonly HtmlDocument document = new HtmlDocument();
        public void Load(string html)
        {
            HtmlNode.ElementsFlags.Remove("form");
            document.LoadHtml(html);
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

                return document.DocumentNode.SelectSingleNode("//form[translate(@action,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" + action + "' " +
                "and translate(@method,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" + method + "']");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public HtmlNodeCollection GetForms(Dictionary<string, string> attribs = null)
        {
            if (attribs == null)
                attribs = new Dictionary<string, string>();
            try
            {
                if (attribs.Count == 0)
                {
                    var ret = document.DocumentNode.SelectNodes("//form");
                    if (ret == null)
                        return new HtmlNodeCollection(document.DocumentNode);
                    return ret;
                }
                var restrict = "";
                foreach (var key in attribs.Keys)
                {
                    restrict += "translate(@" + key + ",'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" + attribs[key].ToLower() + "'";
                    restrict += " and ";
                }
                restrict = restrict.Substring(0, restrict.Length - 5);
                var ret1= document.DocumentNode.SelectNodes("//form[" + restrict + "]");
                if (ret1 == null)
                    return new HtmlNodeCollection(document.DocumentNode);
                return ret1;
            }
            catch (Exception ex)
            {
                return new HtmlNodeCollection(document.DocumentNode);
            }
        }
        public HtmlNode GetElementById(string id)
        {
            return document.GetElementbyId(id);
        }
        public HtmlNodeCollection GetNodes(string xpath)
        {
            var ret= document.DocumentNode.SelectNodes(xpath);
            if (ret == null)
                return new HtmlNodeCollection(document.DocumentNode);
            return ret;

        }

        public HtmlNodeCollection GetLinks()
        {
                var ret = document.DocumentNode.SelectNodes("//a");
                if (ret == null)
                    return new HtmlNodeCollection(document.DocumentNode);
                return ret;
            
        }
    }
}
