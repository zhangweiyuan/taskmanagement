using MySql.Data.Entity;
using System.Data.Entity;

namespace Db
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class TaskDbContext : DbContext
    {
        public TaskDbContext() : base("name=mysqlconnection")
        {
            Database.SetInitializer<TaskDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //https://docs.microsoft.com/zh-cn/previous-versions/gg696316(v=vs.113)
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PrimaryKeyNameForeignKeyDiscoveryConvention>();
        }

        public DbSet<Models.Tasks> Tasks { get; set; }
        public DbSet<Models.Users> Users { get; set; }
        public DbSet<Models.Groups> Groups { get; set; }
        public DbSet<Models.GroupUsers> GroupUsers { get; set; }
        public DbSet<Models.Endorses> Endorses { get; set; }
        public DbSet<Models.Remind> Remind { get; set; }
    }
}