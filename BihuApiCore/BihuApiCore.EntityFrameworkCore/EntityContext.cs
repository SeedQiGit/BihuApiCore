using BihuApiCore.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.EntityFrameworkCore
{
    public class EntityContext:DbContext
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {
            Database.EnsureCreated();
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<User> Products { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
