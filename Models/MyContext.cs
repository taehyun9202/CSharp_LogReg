using Microsoft.EntityFrameworkCore;
namespace RegLog.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Members {get;set;}
    }
}