using Microsoft.EntityFrameworkCore;

namespace WebAppCAPDemo
{
    public class CAPDbcontext : DbContext
    {
        public CAPDbcontext(DbContextOptions<CAPDbcontext> dbContextOptions) : base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<test>().HasKey(x => new { x.Name, x.Age });
        }

        public class test
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
