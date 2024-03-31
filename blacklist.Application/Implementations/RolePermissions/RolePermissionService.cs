using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blacklist.Application.Interfacses.RolePermissions;

namespace blacklist.Application.Implementations.RolePermissions
{
    public class RolePermissionService : ResponseBaseService, IRolePermission
    {
        private readonly IMessageProvider _messageProvider;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAppDbContext _context;
        private readonly string _language;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDbContextTransaction _trans;
        public RolePermissionService(IMessageProvider messageProvider, IHttpContextAccessor httpContext, RoleManager<ApplicationRole> roleManager, IAppDbContext context) : base(messageProvider)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _trans = _context.Begin();

        }

        public async Task<ServerResponse<bool>> AssignPermissionsToRole(RolePermissionDTO request)
        {
            var response = new ServerResponse<bool>();
            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetError(response, source.Code, source.Message, _language);
            }

           
                var role = _context.Roles.Find(request.RoleId);
                if (role == null)
                {
                    return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
                }

                foreach (var permissionId in request.PermissionIds)
                {
                    var permission = _context.Permissions.Find(permissionId);

                    if (permission == null)
                    {
                        return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
                    }

                    var existingAssignment = _context.RolePermissions
                        .Find(role.Id, permissionId);

                    if (existingAssignment != null)
                    {
                        return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
                    }

                    var rolePermission = new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permissionId,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        IsActive = true
                        
                    };

                    _context.RolePermissions.Add(rolePermission);
                }

                _context.SaveChanges();
                await _trans.CommitAsync();
                response.IsSuccessful = true;

                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
            

            return response;
        }


        public async Task<ServerResponse<bool>> UnassignPermissionsFromRole(RolePermissionDTO request)
        {
            var response = new ServerResponse<bool>();
            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetError(response, source.Code, source.Message, _language);
            }

           
                // Ensure that the role exists in your database
                var role = _context.Roles.Find(request.RoleId);

                if (role == null)
                {
                    return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
                }

                // Find the existing RolePermission assignments to remove
                var assignmentsToRemove = _context.RolePermissions
                    .Where(rp => rp.RoleId == role.Id && request.PermissionIds.Contains(rp.PermissionId))
                    .ToList();

                if (assignmentsToRemove.Any())
                {
                    _context.RolePermissions.RemoveRange(assignmentsToRemove);
                    await _context.SaveChangesAsync();
                    await _trans.CommitAsync();

                    response.IsSuccessful = true;
                    SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
                }
                else
                {
                    // No matching assignments found
                    return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
                }
        

            return response;
        }

        public async Task<ServerResponse<bool>> UpdateRolePermissions(RolePermissionUpdateDto request)
        {
            var response = new ServerResponse<bool>();

            var role = _context.Roles.Find(request.RoleId);
            if (role == null)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
            }

            // Find the existing RolePermission assignments for the role
            var existingAssignments = _context.RolePermissions
                .Where(rp => rp.RoleId == request.RoleId)
                .ToList();

            if(existingAssignments == null || existingAssignments.Count == 0)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
            }

            var permissionsToRemove = request.RemovePermissionIds?.Any() == true
                ? existingAssignments.Where(rp => request.RemovePermissionIds.Contains(rp.PermissionId)).ToList()
                : new List<RolePermission>();

            // Find new permissions to add
            var newPermissionsToAdd = request.AddPermissionIds?.Any(id => id != 0) == true
                ? request.AddPermissionIds.Where(permissionId => existingAssignments.All(rp => rp.PermissionId != permissionId)).ToList()
                : new List<long>();

            try
             {
                // Remove permissions
                if (permissionsToRemove.Any())
                {
                    _context.RolePermissions.RemoveRange(permissionsToRemove);
                }

                // Add new permissions
                foreach (var permissionId in newPermissionsToAdd)
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = request.RoleId,
                        PermissionId = permissionId,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        IsActive = true
                    };

                    _context.RolePermissions.Add(rolePermission);
                }

                // Save changes
                await _context.SaveChangesAsync();
                await _trans.CommitAsync();

                response.IsSuccessful = true;
                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or set response error information
                response.IsSuccessful = false;
                response.SuccessMessage = "An error occurred while updating role permissions.";
                // LogError(ex.Message);
            }

            return response;
        }


        public async Task<ServerResponse<List<RolePermissionModel>>> GetAllRecord()
        {
            var response = new ServerResponse<List<RolePermissionModel>>();

            try
            {
                var rolePermissions = await _context.RolePermissions
                    .Include(rp => rp.Permission)
                    .GroupBy(rp => rp.RoleId)
                    .Select(group => new RolePermissionModel
                    {
                        RoleId = group.Key,
                        Permissions = group.Select(rp => new PermissionModels
                        {
                            PermissionId = rp.PermissionId,
                            PermissionName = rp.Permission.Name
                        }).ToList()
                    })
                    .ToListAsync();

                response.IsSuccessful = true;
                response.Data = rolePermissions;
                response.SuccessMessage = "Role and permission successfully retrieved";
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or set response error information
                response.IsSuccessful = false;
                response.SuccessMessage = "An error occurred while fetching records.";
                // LogError(ex.Message);
            }

            return response;
        }


        public async Task<ServerResponse<List<RolePermissionModel>>> GetRecordById(string roleId)
        {
            var response = new ServerResponse<List<RolePermissionModel>>();

            try
            {
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId) // Filter by roleId
                    .Include(rp => rp.Permission)
                    .GroupBy(rp => rp.RoleId)
                    .Select(group => new RolePermissionModel
                    {
                        RoleId = group.Key,
                        Permissions = group.Select(rp => new PermissionModels
                        {
                            PermissionId = rp.PermissionId,
                            PermissionName = rp.Permission.Name
                        }).ToList()
                    })
                    .ToListAsync();

                response.IsSuccessful = true;
                response.Data = rolePermissions;
                response.SuccessMessage = "Role and permission successfully retrieved";
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, or set response error information
                response.IsSuccessful = false;
                response.SuccessMessage = "An error occurred while fetching records.";
                // LogError(ex.Message);
            }

            return response;
        }
 


    }
}
