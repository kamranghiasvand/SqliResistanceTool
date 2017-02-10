using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SqliResistanceModel;

namespace SqliResistanceTool
{

    public class SqliConfig : ConfigurationSection
    {
        [ConfigurationProperty("Sites", IsDefaultCollection = true)]
        public SiteCollection Sites
        {
            get { return (SiteCollection)this["Sites"]; }
        }
    }
    [ConfigurationCollection(typeof(SiteToProcess),AddItemName = "SiteToProcess")]
    public class SiteCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SiteToProcess();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SiteToProcess)element).SiteUri;
        }
    }
    public class SiteToProcess : ConfigurationElement
    {
        [ConfigurationProperty("SiteUri", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string SiteUri
        {
            get { return (string)this["SiteUri"]; }
            set { this["SiteRoot"] = value; }

        }
        [ConfigurationProperty("LoginInformation", IsRequired = false, DefaultValue = null)]
        public LoginElement Login
        {
            get { return (LoginElement)this["LoginInformation"]; }
            set { this["LoginInformation"] = value; }
        }

    }

    public class LoginElement : ConfigurationElement
    {
        [ConfigurationProperty("LoginUri", IsRequired = false)]
        public string LoginUri
        {
            get { return (string)this["LoginUri"]; }
            set { this["LoginUri"] = value; }
        }
        [ConfigurationProperty("LoginData", IsDefaultCollection = true)]
        public LoginDataCollection LoginData
        {
            get { return (LoginDataCollection)this["LoginData"]; }
        }
        [ConfigurationProperty("LoginButton", IsRequired = false)]
        public SearchUiElement LoginButton
        {
            get { return (SearchUiElement)this["LoginButton"]; }
            set { this["LoginButton"] = value; }
        }
    }
    [ConfigurationCollection(typeof(SiteToProcess))]
    public class LoginDataCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoginData();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoginData)element).Key;
        }
    }
    public class LoginData : ConfigurationElement
    {
        [ConfigurationProperty("Key",IsRequired  =true)]
        public string Key
        {
            get { return (string)this["Key"]; }
            set { this["Key"] = value; }
        }
        [ConfigurationProperty("Value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["Value"]; }
            set { this["Value"] = value; }
        }
    }
    public class SearchUiElement : ConfigurationElement
    {
        [ConfigurationProperty("By", DefaultValue = SearchBy.Name, IsRequired = true)]
        public SearchBy By
        {
            get { return (SearchBy)this["By"]; }
            set { this["By"] = value; }
        }
        [ConfigurationProperty("Value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["Value"]; }
            set { this["Value"] = value; }
        }

    }
}
