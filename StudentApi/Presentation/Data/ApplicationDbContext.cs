using Microsoft.EntityFrameworkCore;
using DefaultNamespace.Models; 

namespace DefaultNamespace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }


       
    }
}