using Microsoft.EntityFrameworkCore;
using AnyLearnServer.Models;

namespace AnyLearnServer.Database
{
    public class AnyLearnDatabaseContext : DbContext
    {

        public DbSet<User>? Users { get; set; }

    }
}
