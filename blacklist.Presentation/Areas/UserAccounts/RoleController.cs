using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using blacklist.Application.Interfacses.Roles;
using blacklist.Presentation.Controllers;

namespace blacklist.Api.Controllers
{
    [Authorize]
    [Area("UserAccounts")]
    [Route("api/roles")]
    public class RoleController : BaseAPIController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [ProducesResponseType(typeof(ServerResponse<List<RoleModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("view-role")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAllRecord();

            if (response.IsSuccessful)
            {
                return Ok( response );
            }
            return BadRequest(  response.Error);

        }

        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("create-role")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(RoleDTO request)
        {
            var response = await _roleService.Create(request);

            
            if (response.IsSuccessful)
            {

                return Ok( response );
            }
            else
            {
                return BadRequest( response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("update-role")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(RoleDTO request)
        {
            var response = await _roleService.Update(request);
 
            if (response.IsSuccessful)
            {

                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("delete-role")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _roleService.Delete(id);
 
            if (response.IsSuccessful)
            {

                return Ok(response );
            }
            else
            {
                return BadRequest(  response.Error);
            }
        }

        [ProducesResponseType(typeof(ServerResponse<RoleModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("view-role/{id}")]
        //[Authorize(Roles = "Developer")]
        public async Task<IActionResult> ViewRoleById([FromRoute] string id)
        {

            var response = await _roleService.GetRecordById(id);
            
            if (response.IsSuccessful)
            {

                return Ok( response );
            }
            else
            {
                return BadRequest(  response .Error);
            }
        }
    }
}
