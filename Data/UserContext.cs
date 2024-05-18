using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace authentication.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        public DbSet<User> Users { set; get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });

        }
    }
}