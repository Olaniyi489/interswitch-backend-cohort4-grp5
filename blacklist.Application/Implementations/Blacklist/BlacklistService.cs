using Azure.Core;
using blacklist.Application.Interfacses.Blacklist;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Implementations.Blacklist
{
    public class BlacklistService : ResponseBaseService, IBlacklistService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IMessageProvider _messageProvider;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IAppDbContext _context;
        private readonly IDbContextTransaction _trans;
        private readonly string _language;
        private readonly IEmailService _emailService;
        private readonly IHostEnvironment _environment;
        private readonly EmailHelper _emailHelper;
        private readonly EmailServiceBinding _emailServiceBinding;
        private readonly ITokenService _tokenService;
        public BlacklistService(IMessageProvider messageProvider,
            IHttpContextAccessor httpContext,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IEmailService emailService,
            IAppDbContext context,
            IHostEnvironment hostEnvironment,
            RoleManager<ApplicationRole> roleManager,
            EmailHelper emailHelper,
            EmailServiceBinding emailServiceBinding,
            ITokenService tokenService, ILoggerFactory log) : base(messageProvider)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _config = config ?? throw new ArgumentNullException(nameof(config)); ;
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _trans = _context.Begin();
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _emailService = emailService ?? throw new ArgumentNullException("emailService");
            _environment = hostEnvironment ?? throw new ArgumentNullException("hostEnvironment");
            _emailHelper = emailHelper ?? throw new ArgumentNullException("emailHelper");
            _emailServiceBinding = emailServiceBinding ?? throw new ArgumentNullException("emailServiceBinding");
            _tokenService = tokenService ?? throw new ArgumentNullException("tokenService");
            _logger = log.CreateLogger<UserService>();
        }

        public async Task<ServerResponse<bool>> CreateBlacklistByCategory(BlacklistDto request)
        {
            var response = new ServerResponse<bool>();
            var userToken = _tokenService.GetToken();
            _logger.LogInformation($"Inside Create Blacklist by Category for {request.ToString()} {JsonConvert.SerializeObject(request)}");
            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetErrorValidation(response, source.Code, source.Message);
            }

            var userEmail = userToken.User.Email;
            var permission = await GetUserPermissionsAsync(userEmail);
            if (!permission.Any(p => p.PermissionName.Contains("Create_Blacklist")))
            {
                return SetError(response, ResponseCodes.UNAUTHORIZED, _language);
            }

            if (request.Category == BlacklistCategory.Technologies)
            {
                var usersInCategory = await _context.Users
              .Where(u => u.Department == request.Category.ToString())
              .ToListAsync();

                foreach (var user in usersInCategory)
                {
                    var isUserBlacklisted = await _context.Blacklists.AnyAsync(x => x.Email == user.Email);

                    if (isUserBlacklisted)
                    {
                        _logger.LogInformation($"User {user.Email} is already blacklisted. Skipping...");
                        continue;
                    }

                    var blacklistUser = request.Adapt<Domain.Entities.Blacklist>();
                    blacklistUser.Category = request.Category.ToString();
                    blacklistUser.DateCreated = DateTime.Now;
                    blacklistUser.BlacklistedAt = DateTime.Now;
                    blacklistUser.BlacklistedUserId = user.Id;
                    blacklistUser.BlacklistedById = userToken.User.Id;
                    blacklistUser.Email = user.Email;
                    blacklistUser.DateModified = DateTime.Now;

                    await _context.Blacklists.AddAsync(blacklistUser);
                    await _context.SaveChangesAsync();

                    await CreateBlacklistByEmailInternal(user.Email);
                }

            }
            else if (request.Category == BlacklistCategory.Operations)
            {
                var usersInCategory = await _context.Users
             .Where(u => u.Department == request.Category.ToString())
             .ToListAsync();

                foreach (var user in usersInCategory)
                {
                    var isUserBlacklisted = await _context.Blacklists.AnyAsync(x => x.Email == user.Email);

                    if (isUserBlacklisted)
                    {
                        _logger.LogInformation($"User {user.Email} is already blacklisted. Skipping...");
                        continue;
                    }
                    var blacklistUser = request.Adapt<Domain.Entities.Blacklist>();
                    blacklistUser.Category = request.Category.ToString();
                    blacklistUser.DateCreated = DateTime.Now;
                    blacklistUser.BlacklistedAt = DateTime.Now;
                    blacklistUser.BlacklistedUserId = user.Id;
                    blacklistUser.BlacklistedById = userToken.Id;
                    blacklistUser.Email = user.Email;
                    blacklistUser.DateModified = DateTime.Now;

                    await _context.Blacklists.AddAsync(blacklistUser);
                    await _context.SaveChangesAsync();

                    await CreateBlacklistByEmailInternal(user.Email);
                }
            }
            else if (request.Category == BlacklistCategory.Administratives)
            {
                var usersInCategory = await _context.Users
            .Where(u => u.Department == request.Category.ToString())
            .ToListAsync();
                
                foreach (var user in usersInCategory)
                {
                    var isUserBlacklisted = await _context.Blacklists.AnyAsync(x => x.Email == user.Email);

                    if (isUserBlacklisted)
                    {
                        _logger.LogInformation($"User {user.Email} is already blacklisted. Skipping...");
                        continue;
                    }
                    var blacklistUser = request.Adapt<Domain.Entities.Blacklist>();
                    blacklistUser.Category = request.Category.ToString();
                    blacklistUser.DateCreated = DateTime.Now;
                    blacklistUser.BlacklistedAt = DateTime.Now;
                    blacklistUser.BlacklistedUserId = user.Id;
                    blacklistUser.BlacklistedById = userToken.Id;
                    blacklistUser.Email = user.Email;
                    blacklistUser.DateModified = DateTime.Now;

                    await _context.Blacklists.AddAsync(blacklistUser);
                    await _context.SaveChangesAsync();

                    await CreateBlacklistByEmailInternal(user.Email);
                }

            }
            else if (request.Category == BlacklistCategory.All)
            {
                var usersInCategory = await _context.Users
                .ToListAsync();

                foreach (var user in usersInCategory)
                {
                    var isUserBlacklisted = await _context.Blacklists.AnyAsync(x => x.Email == user.Email);

                    if (isUserBlacklisted)
                    {
                        _logger.LogInformation($"User {user.Email} is already blacklisted. Skipping...");
                        continue;
                    }
                    var blacklistUser = request.Adapt<Domain.Entities.Blacklist>();
                    blacklistUser.Category = request.Category.ToString();
                    blacklistUser.DateCreated = DateTime.Now;
                    blacklistUser.BlacklistedAt = DateTime.Now;
                    blacklistUser.BlacklistedUserId = user.Id;
                    blacklistUser.BlacklistedById = userToken.Id;
                    blacklistUser.Email = user.Email;
                    blacklistUser.DateModified = DateTime.Now;

                    await _context.Blacklists.AddAsync(blacklistUser);
                    await _context.SaveChangesAsync();

                    await CreateBlacklistByEmailInternal(user.Email);
                }
            }
              
                    await _trans.CommitAsync();
                    response.IsSuccessful = true;
                    SetSuccess(response, true, ResponseCodes.SUCCESS, _language);

                   return response;
            
        }

        public async Task<ServerResponse<bool>> CreateBlacklistByEmail(BlackListUserDto request)
        {
            var response = new ServerResponse<bool>();
            var userToken = _tokenService.GetToken();
            _logger.LogInformation($"Inside Create Blacklist by Email for {request.ToString()} {JsonConvert.SerializeObject(request)}");
            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetErrorValidation(response, source.Code, source.Message);
            }

            var userEmail = userToken.User.Email;
            var permission = await GetUserPermissionsAsync(userEmail);
            if (!permission.Any(p => p.PermissionName.Contains("Create_Blacklist")))
            {
                return SetError(response, ResponseCodes.UNAUTHORIZED, _language);
            }

            var userToBlacklist = await _userManager.FindByEmailAsync(request.Email);
            if (userToBlacklist == null)
            {
                return SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);
            }

            var isUserBlacklisted = await _context.Blacklists.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
            if (isUserBlacklisted != null)
            {
                response.IsSuccessful = false;
                return SetError(response, ResponseCodes.USER_ALREADY_BLACKLISTED, _language);
            }
            var blacklistUser = request.Adapt<Domain.Entities.Blacklist>();
            blacklistUser.Category = userToBlacklist.Department;
            blacklistUser.DateCreated = DateTime.Now;
            blacklistUser.BlacklistedAt = DateTime.Now;
            blacklistUser.BlacklistedUserId = userToBlacklist.Id;
            blacklistUser.BlacklistedById = userToken.User.Id;
            blacklistUser.Email = userToBlacklist.Email;
            blacklistUser.DateModified = DateTime.Now;
            await _context.Blacklists.AddAsync(blacklistUser);
            await _context.SaveChangesAsync();

            await CreateBlacklistByEmailInternal(request.Email);

            await _trans.CommitAsync();
            response.IsSuccessful = true;
            SetSuccess(response, true, ResponseCodes.SUCCESS, _language);

            return response;
        }
        
        public async Task<ServerResponse<Domain.Entities.Blacklist>> GetBlacklistedUserByEmail(string email)
        {
            var response = new ServerResponse<Domain.Entities.Blacklist>();
            var userToken = _tokenService.GetToken();

            _logger.LogInformation($"Inside Get Blacklisted User by Email for email: {email}");

            if (string.IsNullOrEmpty(email))
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);

            }

            var userEmail = userToken.User.Email;
            var permission = await GetUserPermissionsAsync(userEmail);
            if (!permission.Any(p => p.PermissionName.Contains("Read_Blacklist")))
            {
                return SetError(response, ResponseCodes.UNAUTHORIZED, _language);
            }

            var blacklistedUser = await _context.Blacklists.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (blacklistedUser == null)
            {
                response.IsSuccessful = false;
                return SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);
            }
            else
            {
                response.IsSuccessful = true;
                response.Data = blacklistedUser;
                SetSuccess(response, blacklistedUser, ResponseCodes.SUCCESS, _language);
            }

            return response;
           
        }

        public async Task<ServerResponse<List<Domain.Entities.Blacklist>>> GetAllBlasklistedUsers()
        {
            var response = new ServerResponse<List<Domain.Entities.Blacklist>>();
            var userToken = _tokenService.GetToken();

            _logger.LogInformation("Inside Get All Blacklisted Users");

            var userEmail = userToken.User.Email;
            var permission = await GetUserPermissionsAsync(userEmail);
            if (!permission.Any(p => p.PermissionName.Contains("Read_Blacklist")))
            {
                return SetError(response, ResponseCodes.UNAUTHORIZED, _language);
            }

            var blacklistedUsers = await _context.Blacklists.ToListAsync();

            if (blacklistedUsers == null || blacklistedUsers.Count == 0)
            {
                response.IsSuccessful = false;
                return SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);
            }
            else
            {
                response.IsSuccessful = true;
                response.Data = blacklistedUsers;
                SetSuccess(response, blacklistedUsers, ResponseCodes.SUCCESS, _language);
            }

            return response;
        }


        public async Task<ServerResponse<List<Domain.Entities.Blacklist>>> GetBlacklistedUserByCategory(string category)
        {
            var response = new ServerResponse<List<Domain.Entities.Blacklist>>();
            var userToken = _tokenService.GetToken();

            _logger.LogInformation("Inside Get All Blacklisted Users");

            var userEmail = userToken.User.Email;
            var permission = await GetUserPermissionsAsync(userEmail);
            if (!permission.Any(p => p.PermissionName.Contains("Read_Blacklist")))
            {
                return SetError(response, ResponseCodes.UNAUTHORIZED, _language);
            }
            
            var blacklistedUsers = new List<Domain.Entities.Blacklist>();    

            if (category.ToString() == BlacklistCategory.Technologies.ToString())
            {
                blacklistedUsers = await _context.Blacklists.Where(x => x.Category == category.ToString()).ToListAsync();
            }else if (category.ToString() == BlacklistCategory.Operations.ToString())
            {
                blacklistedUsers = await _context.Blacklists.Where(x => x.Category == category.ToString()).ToListAsync();
            }else if(category.ToString() == BlacklistCategory.Administratives.ToString())
            {
                blacklistedUsers = await _context.Blacklists.Where(x => x.Category == category.ToString()).ToListAsync();
            }else if(category.ToString() == BlacklistCategory.All.ToString())
            {

                blacklistedUsers = await _context.Blacklists.ToListAsync();
            }
           

            if (blacklistedUsers == null || blacklistedUsers.Count == 0)
            {
                response.IsSuccessful = false;
                return SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);
            }
            else
            {
                response.IsSuccessful = true;
                response.Data = blacklistedUsers;
                SetSuccess(response, blacklistedUsers, ResponseCodes.SUCCESS, _language);
            }

            return response;
        }

        public async Task<ServerResponse<bool>> DeleteBlacklistUser(string email)
        {
            var response = new ServerResponse<bool>();

            var userToken = _tokenService.GetToken();

            _logger.LogInformation("Inside Delete Blacklisted Users");

            var userEmail = userToken.User.Email;
            var permission = await GetUserPermissionsAsync(userEmail);
            if (!permission.Any(p => p.PermissionName.Contains("Delete_Blacklist")))
            {
                return SetError(response, ResponseCodes.UNAUTHORIZED, _language);
            }
            var blacklistedUser = await _context.Blacklists.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (blacklistedUser == null)
            {
                response.IsSuccessful = false;
                return SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);
            }

            _context.Blacklists.Remove(blacklistedUser);
            

            var user = await _userManager.FindByEmailAsync(email);

            user.IsBlacklisted = false;
            user.IsActive = true;

            _context.Users.Update(user);

            int save = _context.SaveChanges();
            if (save <= 0)
            {
                await _trans.RollbackAsync();
                return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            await _trans.CommitAsync();
            response.IsSuccessful = true;
            SetSuccess(response, true, ResponseCodes.SUCCESS, _language);

            return response;
        }

        public async Task<ServerResponse<bool>> DeleteBlacklistedUsers(BlacklistCategory category)
        {
            var response = new ServerResponse<bool>();
            var userToken = _tokenService.GetToken();

            _logger.LogInformation($"Inside Delete Blacklisted Users for category: {category}");

            var userEmail = userToken.User.Email;
            var permission = await GetUserPermissionsAsync(userEmail);
            if (!permission.Any(p => p.PermissionName.Contains("Delete_Blacklist")))
            {
                return SetError(response, ResponseCodes.UNAUTHORIZED, _language);
            }

            if (BlacklistCategory.Technologies == category)
            {
                var blacklistedUsers = await _context.Blacklists.Where(b => b.Category == category.ToString()).ToListAsync();

                _context.Blacklists.RemoveRange(blacklistedUsers);

                foreach (var blacklistedUser in blacklistedUsers)
                {
                    var user = await _userManager.FindByEmailAsync(blacklistedUser.Email);
                    if (user != null)
                    {
                        user.IsBlacklisted = false;
                        user.IsActive = true;
                        _context.Users.Update(user);
                    }
                }
            }
            else if (BlacklistCategory.Operations == category)
            {
                var blacklistedUsers = await _context.Blacklists.Where(b => b.Category == category.ToString()).ToListAsync();

                _context.Blacklists.RemoveRange(blacklistedUsers);

                foreach (var blacklistedUser in blacklistedUsers)
                {
                    var user = await _userManager.FindByEmailAsync(blacklistedUser.Email);
                    if (user != null)
                    {
                        user.IsBlacklisted = false;
                        user.IsActive = true;
                        _context.Users.Update(user);
                    }
                }
            }
            else if (BlacklistCategory.Administratives == category)
            {
                var blacklistedUsers = await _context.Blacklists.Where(b => b.Category == category.ToString()).ToListAsync();

                _context.Blacklists.RemoveRange(blacklistedUsers);

                foreach (var blacklistedUser in blacklistedUsers)
                {
                    var user = await _userManager.FindByEmailAsync(blacklistedUser.Email);
                    if (user != null)
                    {
                        user.IsBlacklisted = false;
                        user.IsActive = true;
                        _context.Users.Update(user);
                    }
                }
            }
            else if (BlacklistCategory.All == category)
            {
                var blacklistedUsers = await _context.Blacklists.ToListAsync();

                _context.Blacklists.RemoveRange(blacklistedUsers);

                foreach (var blacklistedUser in blacklistedUsers)
                {
                    var user = await _userManager.FindByEmailAsync(blacklistedUser.Email);
                    if (user != null)
                    {
                        user.IsBlacklisted = false;
                        user.IsActive = true;
                        _context.Users.Update(user);
                    }
                }
            }
            int save = await _context.SaveChangesAsync();
            if (save <= 0)
            {
                await _trans.RollbackAsync();
                return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            await _trans.CommitAsync();
            response.IsSuccessful = true;
            SetSuccess(response, true, ResponseCodes.SUCCESS, _language);

            return response;
        }

        public async Task<ServerResponse<bool>> UpdateBlacklist(BlacklistDto request)
        {
            var response = new ServerResponse<bool>();
            var userToken = _tokenService.GetToken();

            _logger.LogInformation($"Inside Create Blacklist by Email for {request.ToString()} {JsonConvert.SerializeObject(request)}");

            var userEmail = userToken.User.Email;
            var permission = await GetUserPermissionsAsync(userEmail);
            if (!permission.Any(p => p.PermissionName.Contains("Update_Blacklist")))
            {
                return SetError(response, ResponseCodes.UNAUTHORIZED, _language);
            }

            var user = await _context.Blacklists.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                response.IsSuccessful = false;
                return SetError(response, ResponseCodes.RECORD_DOES_NOT_EXISTS, _language);
            }

            user.Category = request.Category.ToString();
            user.Reason = request.Reason;
            user.BlacklistedAt = request.BlacklistedAt;
            user.Email = request.Email;
            user.BlacklistedById = request.BlacklistedById; 

            var updateResult = _context.Blacklists.Update(user);

            int save = await _context.SaveChangesAsync();
            if (save <= 0)
            {
                await _trans.RollbackAsync();
                return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }

            await _trans.CommitAsync();
            response.IsSuccessful = true;
            SetSuccess(response, true, ResponseCodes.SUCCESS, _language);

            return response;
        }


        private async Task CreateBlacklistByEmailInternal(string email)
        {

            await UpdateUserStatus(email, true);
        }

        private async Task UpdateUserStatus(string email, bool isBlacklisted)
        {
            var updateUser = await _userManager.FindByEmailAsync(email);
            if (updateUser != null)
            {
                updateUser.IsBlacklisted = true;
                updateUser.IsActive = false;
                await _userManager.UpdateAsync(updateUser);
            }
        }
        private async Task<List<GetPermissionModel>> GetUserPermissionsAsync(string userEmail)
        {
            var userPermissions = await _context.Users
                .Where(u => u.Email == userEmail)
                .Join(
                    _context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole }
                )
                .Join(
                    _context.RolePermissions,
                    ur => ur.userRole.RoleId,
                    rolePermission => rolePermission.RoleId,
                    (ur, rolePermission) => new { ur.user, ur.userRole, rolePermission }
                )
                .Join(
                    _context.Permissions,
                    urp => urp.rolePermission.PermissionId,
                    permission => permission.Id,
                    (urp, permission) => new GetPermissionModel
                    {
                        PermissionName = permission.Name,
                        RoleId = urp.rolePermission.RoleId
                    }
                )
                .ToListAsync();

            return userPermissions;
        }

       
    }
}
