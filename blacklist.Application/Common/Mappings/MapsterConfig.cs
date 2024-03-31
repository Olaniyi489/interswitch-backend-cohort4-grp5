
using Global.PayUserManagement.Domain.Entities;

namespace blacklist.Application.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            #region   the mappings start here

            //TypeAdapterConfig<ApplicationUser, UserDto>
            //    .NewConfig()
            //    .Map(dest => dest.UserId, src => src.Id);
            //       TypeAdapterConfig<DashBoardDTO, DashBoardDTO>
            //    .NewConfig()
            //    .Map(dest => dest.Project, src => src.Project.ProjectName)
            //    .Map(dest => dest.ParentDocument, src => src.ParentDocument);
            //#endregion Mapping ends here
        


            //TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
#endregion