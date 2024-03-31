using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blacklist.Application.Implementations;

namespace blacklist.Application.Helpers
{
	public class CodeGeneratorHelper: ResponseBaseService
	{
		private readonly IAppDbContext _context;
		private readonly IDbContextTransaction _trans;
		private readonly IHttpContextAccessor _httpContext;
		private readonly string _language;
	 
		public CodeGeneratorHelper(IAppDbContext context,  IHttpContextAccessor httpContext, IMessageProvider message):base(message) 
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_trans = _context.Begin();
			_httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
			_language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
			 
		}

        //public async Task<string> GenerateProjectRefCode(string projectName)
        //{
        //    long projId = 0;

        //    // Order by some property to make the query deterministic
        //    var refCode = await _context.Projects.OrderBy(p => p.Id).LastOrDefaultAsync();

        //    var proj = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectName.Equals(projectName));
        //    if (proj == null)
        //    {
        //        return string.Empty;
        //    }

        //    string refCodeString = string.Empty;
        //    if (refCode is null)
        //    {
        //        projId = 1;
        //    }
        //    else
        //    {
        //        projId = refCode.Id + 1;
        //    }

        //    string projectRefCode = $" {proj.ProjectName.ToUpperInvariant().Substring(0, 1)} {projId.ToString().PadLeft(5, '0')}";

        //    return projectRefCode;
        //}

        public async Task<string> GenerateProjectRefCode(string projectName)
        {

            long projId = 0;

            var refCode = (await _context.GetData<ProcedureResult>("exec SP_GetLastProjectId"))?.FirstOrDefault()?.LastId;
           
             
            string refCodeString = string.Empty;
            if (refCode <= 0 || refCode is null)
            {
                projId = 1;
            }
            else
            {
                projId = refCode.GetValueOrDefault() + 1;

            }


            string projectRefCode = $" {projectName.ToUpperInvariant().Substring(0, 3)}{projId.ToString().PadLeft(5, '0')}";


            return projectRefCode;
        }
    }
}
