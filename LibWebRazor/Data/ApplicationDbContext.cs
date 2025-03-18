using System.Collections.Generic;
using System.Reflection.Emit;
using LibWebRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace LibWebRazor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "ActionRazor", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFiRazor", DisplayOrder = 2 },
                new Category { Id = 3, Name = "HistoryRazor", DisplayOrder = 3 }
                );
        }
    }
}
