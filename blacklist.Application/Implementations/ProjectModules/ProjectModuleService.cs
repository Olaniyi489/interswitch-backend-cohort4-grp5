using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATSOP.Application.Interfacses.ProjectModules;

namespace VATSOP.Application.Implementations.ProjectModules
{
    public class ProjectModuleService:ResponseBaseService, IProjectModuleService
    {
        private readonly IAppDbContext _context;
        private readonly IDbContextTransaction _trans;
        private readonly IHttpContextAccessor _httpContext;
        private readonly string _language;
        private readonly IMessageProvider _messageProvider;
        public ProjectModuleService(IAppDbContext context, IMessageProvider messageProvider, IHttpContextAccessor httpContext) : base(messageProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _trans = _context.Begin();
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        }

        public async Task<ServerResponse<bool>> Create(ProjectModuleDTO request)
        {
            var response = new ServerResponse<bool>();

            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetErrorValidation(response, source.Code, source.Message);
            }
            var data = request.Adapt<ProjectModule>();
            var record = await _context.ProjectModules.FirstOrDefaultAsync(p => p.UserId.Equals(request.UserId) && p.ProjectId.Equals(request.ProjectId));
            if (record != null)
            {
                return SetError(response, ResponseCodes.RECORD_EXISTS, _language);
            }
            await _context.ProjectModules.AddAsync(data);
            int save = await _context.SaveChangesAsync();
           
            return  await Save(response);
        }

        public async Task<ServerResponse<bool>> Delete(object id)
        {
            var response = new ServerResponse<bool>();

            var data = await _context.ProjectModules.FirstOrDefaultAsync(p => p.Id.Equals(id));
            if (data is null)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
            }
            data.IsDeleted = true;
            data.IsActive = false;
            _context.ProjectModules.Update(data);
            return await Save(response);
        }

     

        public async Task<ServerResponse<List<ProjectModuleModel>>> GetAllRecord()
        {
            var response = new ServerResponse<List<ProjectModuleModel>>();
            var data = await _context.GetData<ProjectModuleModel>("Exec [dbo].[SP_GetProjectModules]");
            response.Data = data;
            response.IsSuccessful = true;
            return response;


        }



        public async Task<ServerResponse<ProjectModuleModel>> GetRecordById(object id)
        {
            var response = new ServerResponse<ProjectModuleModel>();
            var data =  await _context.GetData<ProjectModuleModel>("Exec [dbo].[SP_GetProjectModuleById]",new SqlParameter("@id",id)); 
            response.IsSuccessful = true;
            response.Data = data?.FirstOrDefault();
            return response;
        }

        public async Task<ServerResponse<bool>> Update(ProjectModuleDTO request)
        {
            var response = new ServerResponse<bool>();

            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetErrorValidation(response, source.Code, source.Message);
            }
            var data = await _context.ProjectModules.FirstOrDefaultAsync(p => p.Id.Equals(p.Id));
            if (data is null)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
            }
            data.DateModified = DateTime.Now;
            data.UserId = request.UserId;
            data.ProjectId = request.ProjectId;

            _context.ProjectModules.Update(data);
            
            return  await Save(response);
        }
        private async Task<ServerResponse<bool>> Save(ServerResponse<bool> response)
        {
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
