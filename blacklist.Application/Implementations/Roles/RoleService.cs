using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blacklist.Application.Interfacses.Roles;

namespace blacklist.Application.Implementations.Roles
{
    public class RoleService : ResponseBaseService, IRoleService
    {
        private readonly IMessageProvider _messageProvider;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAppDbContext _context;
        private readonly string _language;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDbContextTransaction _trans;
        public RoleService(IMessageProvider messageProvider, IHttpContextAccessor httpContext, RoleManager<ApplicationRole> roleManager, IAppDbContext context) : base(messageProvider)
        {
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _trans = _context.Begin();

        }

        public async Task<ServerResponse<bool>> Create(RoleDTO request)
        {
            var response = new ServerResponse<bool>();
            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                 return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);

            }
            var role = request.Adapt<ApplicationRole>();
            role.Id = Guid.NewGuid().ToString();
            role.IsActive = true; 
            role.DateCreated = DateTime.Now;

            IdentityResult result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {

                await _trans.CommitAsync();
                response.IsSuccessful = true;
                response.SuccessMessage = "Role was created successfully";

            }
            else
            {
                await _trans.RollbackAsync();
                SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }
            return response;

        }

        public async Task<ServerResponse<bool>> Delete(string id)
        {
            var response = new ServerResponse<bool>();
            if (string.IsNullOrWhiteSpace(id))
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);

            }
            var result = await _roleManager.FindByIdAsync(id);
            if (result is null)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
            }
            result.IsDeleted = true;
            result.IsActive = false;
            _context.Roles.Update(result);
            int save = await _context.SaveChangesAsync();
            if (save > 0)
            {
                await _trans.CommitAsync();
                response.IsSuccessful = true;
                SetSuccess(response,true, ResponseCodes.SUCCESS, _language);
            }
            else
            {
                await _trans.RollbackAsync();
                SetError(response,ResponseCodes.REQUEST_NOT_SUCCESSFUL, _language);
            }
            return response;

        }

        public async Task<ServerResponse<List<RoleModel>>> GetAllRecord()
        {
            var response = new ServerResponse<List<RoleModel>>();

          
                var roles = await _context.Roles
                    .Where(r => !r.IsDeleted)
                    .Select(r => new RoleModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Department = r.Department,
                        RoleDescription = r.RoleDescription,
                        DateCreated = r.DateCreated
                    })
                    .ToListAsync();

                response.IsSuccessful = true;
                response.Data = roles;
          

            return response;
        }


        public async Task<ServerResponse<RoleModel>> GetRecordById(string id)
        {
            var response = new ServerResponse<RoleModel>();
            
                var roles = await _context.Roles
                    .Where(r => r.Id == id && r.IsDeleted == false)
                    .Select(r => new RoleModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Department = r.Department,
                        RoleDescription = r.RoleDescription,
                        DateCreated = r.DateCreated
                    })
                    .ToListAsync();

                response.IsSuccessful = true;
                response.Data = roles.FirstOrDefault();
            

            return response;

        }



        public async Task<ServerResponse<bool>> Update(RoleDTO request)
        {
            var response = new ServerResponse<bool>();

            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetError(response, source.Code, source.Message, _language);
            }

           
                var result = await _roleManager.FindByIdAsync(request.Id);

                if (result is null)
                {
                    return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
                }

                result.Name = request.Name;
                result.RoleDescription = request.RoleDescription;
                result.DateCreated = DateTime.Now;
                result.Department = request.Department;

                await _roleManager.UpdateAsync(result);

                await _context.SaveChangesAsync(); 

                await _trans.CommitAsync();

                response.IsSuccessful = true;
                SetSuccess(response, true, ResponseCodes.SUCCESS, _language);
           

            return response;
        }



    }


}
