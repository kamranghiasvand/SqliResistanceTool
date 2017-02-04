using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SqliResistanceModel
{
    public class FormModel
    {
        [Key]
        public int Id { get; set;}
        public virtual PageModel Page { get; set; }
        public virtual ICollection<FormInputModel> Inputs { get; set; } = new List<FormInputModel>();
        public string AcceptCharset { get; set; }
        public string Action { get; set; }
        public string Enctype { get; set; }
        public string Method { get; set; }
        public string Name { get; set; }
        public string Novalidate { get; set; }
        public string Target { get; set; }
    }
}
