using System.ComponentModel.DataAnnotations;

namespace SqliResistanceModel
{
    public class ElementSearchModel
    {
        [Key]
        public int Id { get; set; }
        public SearchBy By { get; set; }
        public string Value { get; set; }
    }

    public enum SearchBy
    {
        ClassName,
        CssSelector,
        Id,
        LinkText,
        PartialLinkText,
        TagName,
        XPath,
        Name

    }
}
