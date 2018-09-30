using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BihuApiCore.EntityFrameworkCore
{
    public class EntityContext:DbContext
    {
        //异步方法使用
        public EntityContext()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        //这两种构造函数，一个是依赖注入，一个是手动新增用的，我这种理解对不？？
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {
            Database.EnsureCreated();
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conStr = ConfigurationManager.GetValue("ConnectionStrings:EntityContext");
                optionsBuilder.UseMySql(conStr);
            }
        }

        public DbSet<User> User { get; set; }
        public DbSet<Product> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
