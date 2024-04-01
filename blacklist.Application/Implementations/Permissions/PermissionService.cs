using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blacklist.Application.Interfacses.Permissions;

namespace blacklist.Application.Implementations.Permissions
{
    public class PermissionService : ResponseBaseService, IPermissionService
    {
        private readonly IMessageProvider _messageProvider;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAppDbContext _context;
        private readonly string _language;

        private readonly IDbContextTransaction _trans;
        public PermissionService(IMessageProvider messageProvider, IHttpContextAccessor httpContext,
             IAppDbContext context) : base(messageProvider)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _trans = _context.Begin();

        }
        public async Task<ServerResponse<bool>> Create(PermissionDTO request)
        {
            var response = new ServerResponse<bool>();
            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);

            }
            var data = request.Adapt<Permission>();
            var result = await _context.Permissions.AddAsync(data);
            int save = await _context.SaveChangesAsync();
            if (save > 0)
            {

                await _trans.CommitAsync();
                response.IsSuccessful = true;
                response.Data = true;
                response.SuccessMessage = "Permission created successfully";
            }
            else
            {
                await _trans.RollbackAsync();
                response.IsSuccessful = false;
                response.Data = false;
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }
            return response;
        }

        public async Task<ServerResponse<bool>> Delete(object id)
        {
            long reqId = Convert.ToInt64(id);
            var response = new ServerResponse<bool>();
            if (reqId <= 0)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);

            }
            var result = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == reqId);

            result.IsDeleted = true;
            _context.Permissions.Update(result);
            int save = await _context.SaveChangesAsync();
            if (save > 0)
            {
                await _trans.CommitAsync();
                response.IsSuccessful = true;
                response.Data = true;
                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
            }
            else
            {
                await _trans.RollbackAsync();
                response.IsSuccessful = false;
                response.Data = false;
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }
            return response;
        }

        public async Task<ServerResponse<Permission>> GetRecordById(object Id)
        {
            var response = new ServerResponse<Permission>();
           
                var data = await _context.Permissions
                    .Where(p => p.Id == (long)Id)
                    .Select(p => new Permission
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        RolePermissions = p.RolePermissions
                    })
                    .ToListAsync();

                response.IsSuccessful = true;
                response.Data = data?.FirstOrDefault();
            

            return response;

        }

        public async Task<ServerResponse<List<Permission>>> GetAllRecord()
        {

            var response = new ServerResponse<List<Permission>>();
          
                var data = await _context.Permissions
                    .Select(p => new Permission
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        RolePermissions = p.RolePermissions
                    })
                    .ToListAsync();

                response.Data = data;
                response.IsSuccessful = true;
          
            
            return response;

        }

        //public async Task<ServerResponse<PermissionModel>> GetRecordById(object id)
        //{
        //    var response = new ServerResponse<PermissionModel>();
        //    var data = await _context.GetData<PermissionModel>("Exec [dbo].[SP_GetPermissionById]", new SqlParameter("@id", id));

        //    var record = data?.Adapt<PermissionModel>();
        //    response.IsSuccessful = true;
        //    response.Data = record;
        //    return response;
        //}

        public async Task<ServerResponse<bool>> Update(PermissionDTO request)
        {

            var response = new ServerResponse<bool>();
            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetError(response, source.Code, source.Message, _language);

            }
            var result = await _context.Permissions.FirstOrDefaultAsync(p => p.Id.Equals(request.Id));
            if (result is null)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
            }
            result.Name = request.Name;
            result.Description = request.Description;

            _context.Permissions.Update(result);

            int save = await _context.SaveChangesAsync();
            if (save > 0)
            {
                await _trans.CommitAsync();
                response.IsSuccessful = true;
                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
            }
            else
            {
                await _trans.RollbackAsync();
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }
            return response;
        }
    }
}
