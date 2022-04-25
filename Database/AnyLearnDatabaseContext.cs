using Microsoft.EntityFrameworkCore;
using AnyLearnServer.Models;

namespace AnyLearnServer.Database
{
    public class AnyLearnDatabaseContext : DbContext
    {
        public AnyLearnDatabaseContext(DbContextOptions opt) : base(opt)
        {
            
        }

        public DbSet<User>? Users { get; set; }

    }
}
