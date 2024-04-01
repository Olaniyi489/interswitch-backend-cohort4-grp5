
using blacklist.Domain.Entities;
using Microsoft.Graph.Me.ManagedDevices.Item.Users;
using blacklist.Application.Common.Enums;


namespace blacklist.Persistence.DataContexts
{
    public partial class BlacklistDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>, IAppDbContext
    {
        public BlacklistDbContext()
        {
        }

        public BlacklistDbContext(DbContextOptions<BlacklistDbContext> options)
            : base(options)
        {
        }

        #region Tables
        public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; } = null!;
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<UserActivities> UserActivities { get; set; }
        public virtual DbSet<Blacklist> Blacklists { get; set; }      
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<OTPs> OTPs { get; set; }
        public virtual DbSet<MessagingSystem> MessagingSystem { get; set; }
      



        #endregion

        #region Procedures
        public virtual DbSet<GetPermissionModel> GetPermissionModels { get; set; }
        public virtual DbSet<ProcedureResult> ProcedureResults { get; set; }
        

        #endregion


        #region Views


        #endregion

        #region Data Helpers

        public async Task<List<T>> GetData<T>(string query, params object[] param) where T : class
        {
            List<T> data = new();
            if (param != null && param.Length > 0)
            {
                var para = string.Join(",", param);
                data = await this.Set<T>().FromSqlRaw($"{query} {para}", param).ToListAsync();
            }
            else
            {
                data = await this.Set<T>().FromSqlRaw(query).ToListAsync();
            }
            return data;
        }
        public IQueryable<T> GetDataQueriable<T>(string query, params object[] param) where T : class
        {
            IQueryable<T> data = null;
            if (param != null && param.Length > 0)
            {
                var para = string.Join(",", param);
                data = this.Set<T>().FromSqlRaw($"{query} {para}", param);
            }
            else
            {
                data = this.Set<T>().FromSqlRaw(query);
            }
            return data;
        }

        public async Task<int> ExecuteSqlCommand(string query, params object[] param)
        {
            var para = string.Join(",", param.ToList());
            var result = await this.ExecuteSqlCommand($"{query} {para}", param);
            return result;
        }

        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            base.OnModelCreating(modelBuilder);


            #region procedures
            //modelBuilder.Entity<FeedLOBSelectListItemModel>(p => p.HasNoKey());
            //modelBuilder.Entity<StandardOperatingProcedureModel>(p => p.HasNoKey());
            //modelBuilder.Entity<ProjectModuleModel>(p => p.HasNoKey());
            //modelBuilder.Entity<PermissionModel>(p => p.HasNoKey());
            //modelBuilder.Entity<ProjectModel>(p => p.HasNoKey());
            modelBuilder.Entity<ProcedureResult>(p => p.HasNoKey());
            modelBuilder.Entity<RolePermission>()
                                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                                .HasOne(rp => rp.Role)
                                .WithMany(r => r.RolePermissions)
                                .HasForeignKey(rp => rp.RoleId);

            #endregion




            #region Tables
            modelBuilder.Entity<OTPs>(o => { o.HasKey(o => o.Id); });
            modelBuilder.Entity<GetPermissionModel>(p => { p.HasKey(p => p.RoleId); });
            //To consume views or procedure 


            modelBuilder.Entity<ApplicationUser>(entity =>
            {

                entity.HasKey(e => e.Id);

                entity.ToTable("Users");

            });
            modelBuilder.Entity<ApplicationRole>(entity =>
            {

                entity.HasKey(e => e.Id);

                entity.ToTable("Roles");

            });
            modelBuilder.Entity<ApplicationUserRole>(entity =>
            {

                entity.ToTable("UserRoles");

            });
            modelBuilder.Entity<ApplicationUserClaim>(entity =>
            {

                entity.ToTable("UserClaims");

            });
            modelBuilder.Entity<ApplicationRoleClaim>(entity =>
            {

                entity.ToTable("RoleClaims");

            });
            modelBuilder.Entity<ApplicationUserLogin>(entity =>
            {

                entity.ToTable("UserLogins");

            });
            modelBuilder.Entity<ApplicationUserToken>(entity =>
            {

                entity.ToTable("UserTokens");

            });

            modelBuilder.Entity<UserRefreshToken>(entity =>
            {
                entity.HasKey(d => d.Id);

            });

            modelBuilder.Entity<Sessions>(entity =>
            {

                entity.HasKey(d => d.Id);
                entity.Property(d => d.Id).ValueGeneratedOnAdd();


            });
            #endregion
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        


        public IDbContextTransaction Begin()
        {
            var trans = this.Database.CurrentTransaction;
            if (this.Database.CurrentTransaction == null)
            {
                trans = this.Database.BeginTransaction();
            }
            return trans;
        }
        public async Task CommitAsync()
        {
            var trans = Begin();

            if (trans != null)
            {

                await trans.CommitAsync();
            }

        }
        public async Task RollbackAsync()
        {
            var trans = Begin();

            if (trans != null)
            {
                await trans.RollbackAsync();
            }

        }
        public DbContext GetAppDbContext()
        {
            return this;
        }
    }
}
