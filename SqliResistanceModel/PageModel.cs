using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqliResistanceModel
{
    public class PageModel
    {
        [Key]
        public int Id { get; set; }
        [DataType(DataType.Url)]
        public string UrlString { get; set; }
        private Uri url;
        [NotMapped]
        public Uri Url
        {
            get { return url ?? (url = new Uri(UrlString)); }
            set
            {
                url = value;
                UrlString = url.AbsoluteUri;
            }
        }
        public DateTime VisitedDate { get; set; }
        public string RawContent { get; set; }
        public virtual ICollection<FormModel> Forms { get; set; } = new List<FormModel>();
    }
}
