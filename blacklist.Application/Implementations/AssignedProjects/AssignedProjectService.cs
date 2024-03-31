using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATSOP.Application.Interfacses.AssignedProjects;

namespace VATSOP.Application.Implementations.AssignedProjects
{
    public class AssignedProjectService:ResponseBaseService, IAssignedProjectService
    {
        private readonly IAppDbContext _context;
        private readonly IDbContextTransaction _trans;
        private readonly IHttpContextAccessor _httpContext;
        private readonly string _language;
        private readonly IMessageProvider _messageProvider;
        public AssignedProjectService(IAppDbContext context, IMessageProvider messageProvider, IHttpContextAccessor httpContext) : base(messageProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _trans = _context.Begin();
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        }

        public async Task<ServerResponse<bool>> Create(AssignedProjectDTO request)
        {
            var response = new ServerResponse<bool>();

            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetErrorValidation(response, source.Code, source.Message);
            }
            var data = request.Adapt<AssignedProject>();
            var record = await _context.AssignedProjects.FirstOrDefaultAsync(p => p.AssignerId.Equals(request.AssignerId) && p.ProjectId.Equals(request.ProjectId));
            if (record != null)
            {
                return SetError(response, ResponseCodes.RECORD_EXISTS, _language);
            }
            await _context.AssignedProjects.AddAsync(data);
            int save = await _context.SaveChangesAsync();

            return await Save(response);
        }

        public async Task<ServerResponse<bool>> Delete(object id)
        {
            var response = new ServerResponse<bool>();

            var data = await _context.AssignedProjects.FirstOrDefaultAsync(p => p.Id.Equals(id));
            if (data is null)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
            }
            data.IsDeleted = true;
            data.IsActive = false;
            _context.AssignedProjects.Update(data);
            return await Save(response);
        }



        public async Task<ServerResponse<List<AssignedProjectModel>>> GetAllRecord()
        {
            var response = new ServerResponse<List<AssignedProjectModel>>();
            var data = await _context.GetData<AssignedProjectModel>("Exec [dbo].[SP_GetAssignedProjects]");
            response.Data = data;
            response.IsSuccessful = true;
            return response;


        }



        public async Task<ServerResponse<AssignedProjectModel>> GetRecordById(object id)
        {
            var response = new ServerResponse<AssignedProjectModel>();
            var data = await _context.GetData<AssignedProjectModel>("exec [dbo].[SP_GetAssignedProjectById]", new SqlParameter("@id", id));

            response.IsSuccessful = true;
            response.Data = data?.FirstOrDefault();
            return response;
        }

        public async Task<ServerResponse<bool>> Update(AssignedProjectDTO request)
        {
            var response = new ServerResponse<bool>();

            if (!request.IsValid(out ValidationResponse source, _messageProvider, _language))
            {
                return SetErrorValidation(response, source.Code, source.Message);
            }
            var data = await _context.AssignedProjects.FirstOrDefaultAsync(p => p.Id.Equals(p.Id));
            if (data is null)
            {
                return SetError(response, ResponseCodes.INVALID_PARAMETER, _language);
            }
            data.DateModified = DateTime.Now;
            data.AssigneeId = request.AssigneeId;
            data.AssignerId = request.AssignerId;

            _context.AssignedProjects.Update(data);

            return await Save(response);
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
