using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blacklist.Application.Interfacses.DashBoard;

namespace blacklist.Application.Implementations.DashBoard
{
    public class DashBoardService: ResponseBaseService,IDashBoardService
    {
        private readonly IAppDbContext _context;
        private readonly IDbContextTransaction _trans;
        private readonly IHttpContextAccessor _httpContext;
        private readonly string _language;
        private readonly IMessageProvider _messageProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashBoardService(IAppDbContext context, IMessageProvider messageProvider, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext) : base(messageProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _trans = _context.Begin();
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _language = _httpContext.HttpContext.Request.Headers[ResponseCodes.LANGUAGE];
            _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
            _userManager = userManager;
        }



        public async Task<ServerResponse<DashBoardDTO>> GetDashDoardDetails(string roleId)
        {
            var response = new ServerResponse<DashBoardDTO>();

            //var totalProject = await _context.Projects.CountAsync();
            //var totalLineOfBussiness = await _context.LineOfBusinesses.CountAsync();
            //var totalDevelopers = await _context.ProjectDevelopers.CountAsync();

            //var totalAssignedProject = await _context.Projects
            //    .Where(p => p.StartDate != null)
            //    .CountAsync();

            //var totalUnassignedProject = await _context.Projects
            //    .Where(p => p.StartDate == null)
            //    .CountAsync();

            //var totalAssignedDevelopers = await _context.ProjectDevelopers
            //    .Where(ad => ad.ProjectId != null)
            //    .CountAsync();

            //var userIds = await _userManager.GetUsersInRoleAsync(roleId);

            //var assignedDeveloperIds = await _context.ProjectDevelopers
            //    .Select(ad => ad.DeveloperId)
            //    .ToListAsync();

            //var totalUnassignedDevelopers = userIds.Count(u => !assignedDeveloperIds.Contains(u.Id));

            //var data = await _context.GetData<LineOfBusiness>("exec [dbo].[SP_GetAllLOBs]");
            //int totalLineOfBusiness = 0; // Initialize to zero
            //var dashBoardDTO = new DashBoardDTO(); // Initialize a new instance

            //if (data != null && data.Any())
            //{
            //    var lineOfBusinessCounts = data
            //        .GroupBy(lob => lob.Name)
            //        .Select(group => new CountLineOfBusiness
            //        {
            //            Name = group.Key,
            //            Count = group.Count()
            //        })
            //        .ToList();

            //    // Sum the counts to get the total
            //    totalLineOfBusiness = lineOfBusinessCounts.Sum(group => group.Count);

            //    // Assign the lineOfBusinessCounts to the LineOfBusinessCount property
            //    dashBoardDTO.LineOfBusinessCount = lineOfBusinessCounts;
            //}
            //else
            //{
            //    response.SuccessMessage = "Line of business does not exist";
            //}

            //dashBoardDTO.TotalProjects = totalProject;
            //dashBoardDTO.TotalDevelopers = totalDevelopers;
            //dashBoardDTO.TotalAssignedProject = totalAssignedProject;
            //dashBoardDTO.TotalUnassignedProject = totalUnassignedProject;
            //dashBoardDTO.TotalAssignedDeveloper = totalAssignedDevelopers;
            //dashBoardDTO.TotalUnAssignedDeveloper = totalUnassignedDevelopers;

            //response.Data = dashBoardDTO;
            //response.IsSuccessful = true;
            //response.SuccessMessage = "dash board details successfully retrieved";

            return response;
        }

    }
}
