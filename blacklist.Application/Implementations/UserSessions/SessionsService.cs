 
namespace blacklist.Application.Implementations.UserSessions
{
    public class SessionsService : ISessionsService
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<SessionsService> _logger;
        private readonly IMessageProvider _messageProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbContextTransaction _trans;
        public SessionsService(IAppDbContext context, ILogger<SessionsService> logger, IMessageProvider messageProvider, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider)); ;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _trans = _context.Begin();
        }

        public async Task<ServerResponse<bool>> CreateSessionAsync(SessionDTO request, string language)
        {
            var response = new ServerResponse<bool>();
            if (!request.IsValid(out ValidationResponse source, _messageProvider, _httpContextAccessor))
            {
                response.Error = new ErrorResponse
                {
                    ResponseCode = source.Code,
                    ResponseDescription = source.Message
                };
                return response;
            }

            var dataMapped = request.Adapt<Sessions>();
            if (dataMapped is null)
            {
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_OBJECT_MAPPING,
                    ResponseDescription = _messageProvider.GetMessage(ResponseCodes.INVALID_OBJECT_MAPPING, language)
                };
                return response;
            }
            dataMapped.DateCreated = DateTime.Now;
            dataMapped.IsActive = true;
            await _context.Sessions.AddAsync(dataMapped);
            int save = await _context.SaveChangesAsync();
            if (save > 0)
            {
                await _trans.CommitAsync();
                response.IsSuccessful = true;
                response.Data = true; response.SuccessMessage = _messageProvider.GetMessage(ResponseCodes.SUCCESS, language);
            }
            else
            {
                response.SuccessMessage = _messageProvider.GetMessage(ResponseCodes.REQUEST_NOT_SUCCESSFUL, language);
            }
            return response;
        }

        public async Task<ServerResponse<bool>> DeleteSessionAsync(string userId, string language)
        {
            var response=new ServerResponse<bool>();
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_PARAMETER,
                    ResponseDescription =_messageProvider.GetMessage(ResponseCodes.INVALID_PARAMETER, language)
                };
                return response;

            }

            var record = await _context.Sessions.FirstOrDefaultAsync(x=>x.UserId.Equals(userId));
            if(record == null)
            {    response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.RECORD_DOES_NOT_EXISTS,
                    ResponseDescription =_messageProvider.GetMessage(ResponseCodes.RECORD_DOES_NOT_EXISTS, language)
                };
                return response;

            }
           _context.Sessions.Remove(record);
            int save = await _context.SaveChangesAsync();
            if (save > 0)
            {
                await _trans.CommitAsync();
                response.Data = true; response.SuccessMessage = _messageProvider.GetMessage(ResponseCodes.SUCCESS, language);
            }
            else
            {
                response.SuccessMessage = _messageProvider.GetMessage(ResponseCodes.REQUEST_NOT_SUCCESSFUL, language);
            }
            return response;
        }

        public async Task<ServerResponse<bool>> UpdateSessionAsync(UpdateSessionDTO request, string language)
        {
            var response = new ServerResponse<bool>();
            if (request.IsValid(out ValidationResponse source, _messageProvider, _httpContextAccessor))
            {
                response.Error = new ErrorResponse
                {
                    ResponseCode = source.Code,
                    ResponseDescription = source.Message
                };
                return response;
            }

            var record = await _context.Sessions.FirstOrDefaultAsync(x => x.UserId.Equals(request.UserId));
            if (record is null)
            {
                response.Error = new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_OBJECT,
                    ResponseDescription = _messageProvider.GetMessage(ResponseCodes.INVALID_OBJECT, language)
                };
                return response;
            }

            record.Token= request.Token;
            _context.Sessions.Update(record);
            int save = await _context.SaveChangesAsync();
            if (save > 0)
            {
                await _trans.CommitAsync();
                response.Data = true; response.SuccessMessage = _messageProvider.GetMessage(ResponseCodes.SUCCESS, language);
            }
            else
            {
                response.SuccessMessage = _messageProvider.GetMessage(ResponseCodes.REQUEST_NOT_SUCCESSFUL, language);
            }
            return response;
        }
    }
}
