using System.Data.Entity;

namespace SqliResistanceModel
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext():base("DefaultConnection")
        {
            
        }
        public DbSet<SiteModel> Sites { get; set; }
    }
}
