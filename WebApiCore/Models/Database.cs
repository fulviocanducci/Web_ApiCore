using Microsoft.EntityFrameworkCore;
namespace WebApiCore.Models
{
    public class Database: DbContext
    {
        public Database(DbContextOptions<Database> options)
            :base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Credit> Credit { get; set; }
    }
}
