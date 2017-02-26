﻿using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace HtmlParser
{
   
    public class XsltCustomContext : XsltContext
    {
        public const string NamespaceUri = "http://XsltCustomContext";

        public XsltCustomContext()
        {
        }

        public XsltCustomContext(NameTable nt)
            : base(nt)
        {
        }

        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
        {
            // Check that the function prefix is for the correct namespace
            if (LookupNamespace(prefix) != NamespaceUri) return null;
            // Lookup the function and return the appropriate IXsltContextFunction implementation
            switch (name)
            {
                case "CaseInsensitiveComparison":
                    return CaseInsensitiveComparison.Instance;
            }

            return null;
        }

        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            return null;
        }

        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return 0;
        }

        public override bool PreserveWhitespace(XPathNavigator node)
        {
            return false;
        }

        public override bool Whitespace => true;

        // Class implementing the XSLT Function for Case Insensitive Comparison
        class CaseInsensitiveComparison : IXsltContextFunction
        {
            private static readonly XPathResultType[] argTypes = { XPathResultType.String };

            public static CaseInsensitiveComparison Instance { get; } = new CaseInsensitiveComparison();

            #region IXsltContextFunction Members

            public XPathResultType[] ArgTypes => argTypes;

            public int Maxargs => 1;

            public int Minargs => 1;

            public XPathResultType ReturnType => XPathResultType.Boolean;

            public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator navigator)
            {
                // Perform the function of comparing the current element to the string argument
                // NOTE: You should add some error checking here.
                string text = args[0] as string;
                return string.Equals(navigator.Value, text, StringComparison.InvariantCultureIgnoreCase);
            }
            #endregion
        }
    }
}
