using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SingalR.Server.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
