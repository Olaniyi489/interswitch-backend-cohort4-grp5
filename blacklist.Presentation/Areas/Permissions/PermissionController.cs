using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using blacklist.Application.Interfacses.Permissions;
using blacklist.Presentation.Controllers;

namespace blacklist.Presentation.Areas.Permissions
{
    [Authorize]
    [Area("Permissions")]
    [Route("api/[controller]")]
    public class PermissionController : BaseAPIController
    {

        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }


        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("create-permission")]
        public async Task<IActionResult> CreatePermission(PermissionDTO request)
        {
            var response = await _permissionService.Create(request);

            
            if (response.IsSuccessful)
            {

                return Ok(  response );
            }
            else
            {
                return BadRequest( response .Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("update-permission")]
        public async Task<IActionResult> UpdatePermission(PermissionDTO request)
        {
            var response = await _permissionService.Update(request);

            
            if (response.IsSuccessful)
            {

                return Ok (response );
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
        [HttpPost("Delete-permission")]
        public async Task<IActionResult> DeletePermission(long id)
        {
            var response = await _permissionService.Delete(id);

            
            if (response.IsSuccessful)
            {

                return Ok( response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<List<Permission>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("get-all-permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var response = await _permissionService.GetAllRecord();

            
            if (response.IsSuccessful)
            {

                return Ok( response );
            }
            else
            {
                return BadRequest( response.Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<Permission>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("get-permission/{id}")]
        public async Task<IActionResult> GetPermissionById(long id)
        {
            var response = await _permissionService.GetRecordById(id);

            
            if (response.IsSuccessful)
            {

                return Ok(  response );
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

    }
}
