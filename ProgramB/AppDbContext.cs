using System.Data.Entity;
using ProgramB.Model;

namespace ProgramB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Hand> Hands { get; set; }
    }
}
