using Microsoft.EntityFrameworkCore;

namespace Net.Core.Authentication.Entities.Contexts
{
    public class AuthContext : DbContext// IdentityDbContext<User>
    {

        public AuthContext(DbContextOptions<AuthContext> options)
           : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        //this function will run when : PM Console>add-migration <name>
        //then : PM Console>update-database
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("user");
            builder.Entity<Account>().ToTable("account");
            builder.Entity<Role>().ToTable("role");
            builder.Entity<UserRole>().ToTable("user_role");
            builder.Entity<Log>().ToTable("log");

            //builder.Entity<User>().ToTable("user").Property(m => m.Id).HasMaxLength(85);
            //builder.Entity<IdentityUser>().ToTable("account").Property(m => m.Id).HasMaxLength(85);
            //builder.Entity<IdentityUser>().ToTable("account").Property(m => m.NormalizedEmail).HasMaxLength(85);
            //builder.Entity<IdentityUser>().ToTable("account").Property(m => m.NormalizedUserName).HasMaxLength(85);

            //builder.Entity<IdentityRole>().ToTable("role").Property(m => m.Id).HasMaxLength(85);
            //builder.Entity<IdentityRole>().ToTable("role").Property(m => m.NormalizedName).HasMaxLength(85);

            //builder.Entity<IdentityUserRole<string>>().ToTable("user_role").Property(m => m.UserId).HasMaxLength(85);
            //builder.Entity<IdentityUserRole<string>>().ToTable("user_role").Property(m => m.RoleId).HasMaxLength(85);

            //builder.Entity<IdentityUserLogin<string>>().ToTable("user_login").Property(m => m.LoginProvider).HasMaxLength(85);
            //builder.Entity<IdentityUserLogin<string>>().ToTable("user_login").Property(m => m.ProviderKey).HasMaxLength(85);
            //builder.Entity<IdentityUserLogin<string>>().ToTable("user_login").Property(m => m.UserId).HasMaxLength(85);

            //builder.Entity<IdentityUserToken<string>>().ToTable("user_token").Property(m => m.UserId).HasMaxLength(85);
            //builder.Entity<IdentityUserToken<string>>().ToTable("user_token").Property(m => m.LoginProvider).HasMaxLength(85);
            //builder.Entity<IdentityUserToken<string>>().ToTable("user_token").Property(m => m.Name).HasMaxLength(85);

            //builder.Entity<IdentityUserClaim<string>>().ToTable("user_claim").Property(m => m.Id).HasMaxLength(85);
            //builder.Entity<IdentityUserClaim<string>>().ToTable("user_claim").Property(m => m.UserId).HasMaxLength(85);

            //builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claim").Property(m => m.Id).HasMaxLength(85);
            //builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claim").Property(m => m.RoleId).HasMaxLength(85);

            //This is the functional for db-migration generate table name
            //foreach (var entityType in builder.Model.GetEntityTypes())
            //{
            //    string newTableNamePrefix = "";
            //    var tableName = entityType.GetTableName();
            //    if (tableName.StartsWith("AspNet"))
            //    {
            //        entityType.SetTableName(newTableNamePrefix + tableName.Substring(6));
            //    }
            //}
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Log> Logs { get; set; }

    }

}
