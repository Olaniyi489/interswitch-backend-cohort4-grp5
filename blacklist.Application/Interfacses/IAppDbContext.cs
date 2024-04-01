








using Global.PayUserManagement.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using blacklist.Domain.Entities;


namespace blacklist.Application.Interfacses
{
    public interface IAppDbContext
    {
        #region Tables
        // DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        DatabaseFacade Database { get; }
        DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Sessions> Sessions { get; set; }
        public DbSet<ApplicationUserRole> UserRoles { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<ApplicationRole> Roles { get; set; }
        public  DbSet<UserActivities> UserActivities { get; set; }      
        public DbSet<Domain.Entities.Blacklist> Blacklists { get; set; }
       
        //public DbSet<ProjectModule> ProjectModules { get; set; }
      
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
     //   public DbSet<ChangeRequest> Requests { get; set; }
        public DbSet<OTPs> OTPs { get; set; }
        public DbSet<MessagingSystem> MessagingSystem { get; set; }
        //public DbSet<RequestComment> RequestComments { get; set; }
        //public DbSet<SupportComment> SupportComments { get; set; }

        #endregion

        #region Stored Procedures
        //public DbSet<FeedLOBSelectListItemModel> FeedLOBSelectListItemModel { get; set; }
        //public DbSet<ProjectReportedIssueModel> ProjectReportedIssueModel { get; set; }
        //public DbSet<ProjectModuleModel> ProjectModuleModel { get; set; }
        //public DbSet<ProjectResolvedIssueModel> ProjectResolvedIssueModel { get; set; }
        //public DbSet<AssignedProjectModel> AssignedProjectModel { get; set; }
        //public DbSet<StandardOperatingProcedureModel> StandardOperatingProcedureModel { get; set; }
        //public DbSet<StandardOperatingProcedureHistoryModel> StandardOperatingProcedureHistoryModel { get; set; }
        //public DbSet<PermissionModel> PermissionModel { get; set; }
        //public DbSet<ProjectModel> ProjectModel { get; set; }

        public DbSet<GetPermissionModel> GetPermissionModels { get; set; }
        public DbSet<ProcedureResult> ProcedureResults { get; set; }
       

        #endregion





        Task<List<T>> GetData<T>(string query, params object[] param) where T : class;
        Task<int> ExecuteSqlCommand(string query, params object[] param);
        IDbContextTransaction Begin();
        Task CommitAsync();
        Task RollbackAsync();
        DbContext GetAppDbContext();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();

    }
}
