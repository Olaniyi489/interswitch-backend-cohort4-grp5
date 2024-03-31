using Microsoft.Identity.Web;
using blacklist.Application.Implementations.DashBoard;
using blacklist.Application.Implementations.RolePermissions;
using blacklist.Application.Interfacses.DashBoard;
using blacklist.Application.Interfacses.RolePermissions;



namespace blacklist.Persistence.ServiceConfigurations
{
    public static class ServiceRegistry
    {
        public static IServiceCollection PushService(this IServiceCollection services, IConfiguration conf)
        {
            //Add services here

            #region Mapster 
            TypeAdapterConfig.GlobalSettings.Default
           .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)
           .IgnoreNullValues(true)
           .AddDestinationTransform((string x) => x.Trim())
           .AddDestinationTransform((string x) => x ?? "")
           .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);
            services.RegisterMapsterConfiguration();

            #endregion

            #region Other services
            services.AddHttpContextAccessor();
            services.AddTokenAcquisition();
            services.AddHttpClient("AccountClient", c => //Named Http Client
            {
                c.DefaultRequestHeaders.Add("language", "en");
            });
			 
			var conString = conf["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<IAppDbContext, BlacklistDbContext>(options => {
                options.UseSqlServer(conString/*, op => { op.EnableRetryOnFailure(); }*/);
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
                options.UseLazyLoadingProxies(false);
              
            }
          
            );
            
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<BlacklistDbContext>()
                .AddDefaultTokenProviders();


			services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISessionsService, SessionsService>();
            services.AddScoped<IAccountLogIn, AccountLogIn>();
            services.AddScoped<IAccountLogout, AccountLogout>();
            services.AddScoped<IntegratedWindowsTokenProvider>(); 
            services.AddScoped<IAccountLogout, AccountLogout>();
            services.AddScoped<IAppDbContext, BlacklistDbContext>();
            services.AddScoped<ILanguageConfigurationProvider, LanguageConfigurationProvider>();
            services.AddScoped<IMessageProvider, MessageProvider>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IResetPassword, ResetPassword>();
            services.AddScoped<IRolePermission, RolePermissionService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            //System Related
            services.AddScoped<IFileSystemManagerService,  FileSystemManagerService>();
            services.AddScoped<IEmailService, EmailService>();
            //Helpers
            services.AddScoped<LanguagePackagePathHelper>();
            services.AddScoped<SystemSettingsHelper>();
            services.AddScoped<MicsrosoftAADHelper>();
            services.AddScoped<ClaimsHelper>();
            services.AddScoped<CodeGeneratorHelper>();
            services.AddScoped<EmailHelper>();
           

            #endregion

            #region Static Files
            services.Configure<LanguageSettings>(options => conf.GetSection("LanguageSettings").Bind(options));
            services.AddSingleton(opt => conf.GetSection("SmtpAccount").Get<SmtpAccount>());
            services.AddSingleton(opt => conf.GetSection("AzureAd").Get<AzureAd>());
            services.AddSingleton(opt => conf.GetSection("EmailServiceBinding").Get<EmailServiceBinding>());

            #endregion

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            #region Plocies and Cors
            
            services.AddCors(options =>
            {

                options.AddPolicy("AllowedPolicy", builder =>
                   builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
            });

			#endregion

		
			return services;
        }
        public static async Task SeedSuperAdmin(this WebApplication app)
        {
            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                var superAdmin = await userManager.FindByEmailAsync("roleuseradmin@gmail.com");
                if (superAdmin == null)
                {
                    superAdmin = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = "roleuseradmin@gmail.com",
                        FirstName = "Role_User",
                        LastName = "Admin",
                        Email = "roleuseradmin@gmail.com",
                        NormalizedEmail = "ROLEUSERADMIN@GMAIL.COM",
                        NormalizedUserName = "ROLEUSERADMIN@GMAIL.COM",
                        PhoneNumber = "1234567890",
                        CountryId = 1,
                        Department = "HR - Application Manager",
                        Occupation = "ROLE_USER_ADMIN",
                        DateCreated = DateTime.Now,
                        IsActive = true,
                        EmailConfirmed = true,
                        FullName= "Role_User Admin"
                    };

                    var password = new PasswordHasher<ApplicationUser>();
                    superAdmin.PasswordHash = password.HashPassword(superAdmin, "Admin@123.");

                    var result = await userManager.CreateAsync(superAdmin);

                    if (result.Succeeded)
                    {
                        var roleExists = await roleManager.RoleExistsAsync("Role_User Admin");
                        if (!roleExists)
                        {
                            var superAdminRole = new ApplicationRole
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Role_User Admin",
                                NormalizedName = "ROLE_USER ADMIN",
                                Department = "Application Role Manager",
                                RoleDescription = "Admin role to manage user permission in the system",
                                DateCreated = DateTime.Now,
                                IsActive = true
                            };
                            await roleManager.CreateAsync(superAdminRole);
                        }

                        await userManager.AddToRoleAsync(superAdmin, "Role_User Admin");
                    }
                }
            }
        }

    }
}