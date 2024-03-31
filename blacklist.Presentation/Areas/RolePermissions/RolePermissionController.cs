using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using blacklist.Application.Interfacses.RolePermissions;
using blacklist.Presentation.Controllers;

namespace blacklist.Presentation.Areas.RolePermission
{
    [Authorize]
    [Area("RolePermission")]
    [Route("api/[controller]")]
    public class RolePermissionController : BaseAPIController
    {
        private readonly IRolePermission _rolePermission;

        public RolePermissionController(IRolePermission rolePermission)
        {
            _rolePermission = rolePermission;
        }


        [ProducesResponseType(typeof(ServerResponse<List<RolePermissionModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("get-all-assigned-permissions")]
        //[Authorize(Roles = "Developer")]
        public async Task<IActionResult> GetAllRecord()
        {

            var response = await _rolePermission.GetAllRecord();
            
            if (response.IsSuccessful)
            {

                return Ok(  response);
            }
            else
            {
                return BadRequest( response.Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<RolePermissionModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpGet("get-role-permissions/{roleId}")]
        //[Authorize(Roles = "Developer")]
        public async Task<IActionResult> GetRecordById([FromRoute] string roleId)
        {

            var response = await _rolePermission.GetRecordById(roleId);
            
            if (response.IsSuccessful)
            {

                return Ok(  response);
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
        [HttpPost("assign-permission")]
        //[Authorize(Roles = "Developer")]
        public async Task<IActionResult> AssignPermission(RolePermissionDTO request)
        {

            var response = await _rolePermission.AssignPermissionsToRole(request);
           
            if (response.IsSuccessful)
            {

                return Ok(  response);
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
        [HttpPost("unassign-permission")]
        //[Authorize(Roles = "Developer")]
        public async Task<IActionResult> UnssignPermission(RolePermissionDTO request)
        {

            var response = await _rolePermission.UnassignPermissionsFromRole(request);
          
            if (response.IsSuccessful)
            {

                return Ok(  response );
            }
            else
            {
                return BadRequest(  response.Error);
            }
        }


        [ProducesResponseType(typeof(ServerResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("update-permission")]
        //[Authorize(Roles = "Developer")]
        public async Task<IActionResult> UpdatePermission(RolePermissionUpdateDto request)
        {

            var response = await _rolePermission.UpdateRolePermissions(request);

            if (response.IsSuccessful)
            {

                return Ok(response);
            }
            else
            {
                return BadRequest(response.Error);
            }
        }

    }
}
