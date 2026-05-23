using AutoMapper;
using CMSBlog.API.Extensions;
using CMSBlog.API.Filters;
using CMSBlog.Core.Constant;
using CMSBlog.Core.Domain.Identity;
using CMSBlog.Core.Models;
using CMSBlog.Core.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMSBlog.API.Controllers.Auth
{
    [Route("api/admin/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        public RoleController(RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        [HttpPost]
        [ValidateModelAttribute]
        [Authorize(Permissions.Roles.Create)]
        public async Task<IActionResult> CreateRole([FromBody] CreateUpdateRoleRequest request)
        {
            await _roleManager.CreateAsync(new AppRole
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
            });
            return Ok();
        }

        [HttpPut("{id}")]
        [ValidateModelAttribute]
        [Authorize(Permissions.Roles.Edit)]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] CreateUpdateRoleRequest request)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }
            role.Name = request.Name;
            role.DisplayName = request.DisplayName;
            await _roleManager.UpdateAsync(role);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Permissions.Roles.Delete)]
        public async Task<IActionResult> DeleteRoles([FromQuery] Guid[] roleIds)
        {
            var roles = await _roleManager.Roles
                .Where(w => roleIds.Contains(w.Id))
                .ToListAsync();
            if (roles.Any())
            {
                foreach (var item in roles)
                {
                    var role = roles.FirstOrDefault(w => w.Id.Equals(item.Id));
                    if (role != null)
                    {
                        await _roleManager.DeleteAsync(role);
                    }
                }
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null) { return NotFound(); }
            var roleModel = _mapper.Map<AppRole>(role);
            return Ok(roleModel);
        }

        [HttpGet]
        [Route("paging")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<PageResult<RoleDto>>> GetRolesAllPaging(string? keyWord, int pageIndex, int pageSize)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(keyWord))
            {
                query = query.Where(w => w.Name.Contains(keyWord) || w.DisplayName.Contains(keyWord));
            }
            var total = query.Count();
            query = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            var data = await _mapper.ProjectTo<RoleDto>(query).ToListAsync();
            var paginated = new PageResult<RoleDto>
            {
                Results = data,
                CurrentPage = pageIndex,
                RowCount = total,
                PageSize = pageSize
            };
            return Ok(paginated);
        }


        [HttpGet("all")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<List<RoleDto>>> GetAll()
        {
            var model = await _mapper.ProjectTo<RoleDto>(_roleManager.Roles).ToListAsync();
            return model;
        }

        [HttpGet("{roleId}/permission")]
        [Authorize(Permissions.Roles.View)]
        public async Task<ActionResult<PermissionDto>> GetAllRolePermission(string roleId)
        {

            var allPermissions = new List<RoleClaimDto>();
            var types = typeof(Permissions).GetNestedTypes();
            foreach (var type in types)
            {
                allPermissions.GetPermission(type);
            }
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
            var model = new PermissionDto
            {
                RoleId = roleId,
            };

            var claims = await _roleManager.GetClaimsAsync(role);
            var allClaimValues = allPermissions.Select(s => s.Value).ToList();
            var roleClaimValues = claims.Select(s => s.Value).ToList();
            var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
            foreach (var permision in allPermissions)
            {
                if (authorizedClaims.Any(a => a == permision.Value))
                {
                    permision.Selected = true;
                }
            }
            model.RoleClaims = allPermissions;
            return Ok(model);
        }

        [HttpPut("permission")]
        [Authorize(Permissions.Roles.Edit)]
        public async Task<IActionResult> SavePermission([FromBody] PermissionDto model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound();
            }
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            var selectedClaims = model.RoleClaims.Where(w => w.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddPermissionClaim(role, claim.Value);
            }
            return Ok();
        }
    }
}
