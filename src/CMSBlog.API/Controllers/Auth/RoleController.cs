using AutoMapper;
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
                foreach (var id in roles)
                {
                    var role = roles.FirstOrDefault(w => w.Id.Equals(id));
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
        public async Task<ActionResult> GetRoleById(Guid id)
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
    }
}
