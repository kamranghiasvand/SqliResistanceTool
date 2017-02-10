using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliResistanceModel
{
    public class FormSelectModel
    {
        [Key]
        public int Id { get; set; }
        public bool Disabled { get; set; }
        public string Name { get; set; }
        public virtual FormModel Form { get; set; }
        public ICollection<string> Values { get; set; } = new List<string>();
    }
}
