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
    [Route("api/admin/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(
            ILogger<UserController> logger,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }
        [HttpGet("{id}")]
        [Authorize(Permissions.Users.View)]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            _logger.LogInformation("[UserController]-[GetUserById]: Getting user with id {UserId}", id);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("[UserController]-[GetUserById]: User with id {UserId} not found", id);
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles.ToList();
            _logger.LogInformation("[UserController]-[GetUserById]: Retrieved user with id {UserId}", id);
            return Ok(userDto);
        }

        [HttpGet("paging")]
        [Authorize(Permissions.Users.View)]
        public async Task<ActionResult<PageResult<UserDto>>> GetUsersPaging(string? keyWord, int pageIndex, int pageSize)
        {
            _logger.LogInformation("[UserController]-[GetUsersPaging]: Getting users with paging parameters {@Request}", new { keyWord, pageIndex, pageSize });
            var query = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(keyWord))
            {
                query = query.Where(u => u.UserName.Contains(keyWord) || u.Email.Contains(keyWord));
            }
            var totalRecords = await query.CountAsync();
            var users = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Roles = roles.ToList();
                userDtos.Add(userDto);
            }
            var pagedResult = new PageResult<UserDto>
            {
                Results = await _mapper.ProjectTo<UserDto>(users).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRecords,
                PageSize = pageSize
            };
            _logger.LogInformation("[UserController]-[GetUsersPaging]: Retrieved {UserCount} users for page {PageIndex}", userDtos.Count, pageIndex);
            return Ok(pagedResult);
        }

        [HttpPost]
        [Authorize(Permissions.Users.Create)]
        [ValidateModel]
        public async Task<ActionResult> CreateUser(CreateUserRequest request)
        {
            _logger.LogInformation("[UserController]-[CreateUser]: Creating user with username {UserName}", request.UserName);
            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Dob = request.Dob,
                Avatar = request.Avatar,
                IsActive = request.IsActive
            };
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            var findUserByName = await _userManager.FindByNameAsync(request.UserName);
            if (findUserByName != null)
            {
                return BadRequest("Username is already taken.");
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                var findUserByEmail = await _userManager.FindByEmailAsync(request.Email);
                if (findUserByEmail != null)
                {
                    return BadRequest("Email is already taken.");
                }
            }
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("[UserController]-[CreateUser]: Failed to create user with username {UserName}. Errors: {@Errors}", request.UserName, result.Errors);
                return BadRequest(string.Join("<br>", result.Errors.Select(x => x.Description)));
            }
            _logger.LogInformation("[UserController]-[CreateUser]: Successfully created user with username {UserName}", request.UserName);
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Permissions.Users.Edit)]
        public async Task<ActionResult> UpdateUser(Guid id, UpdateUserRequest request)
        {
            _logger.LogInformation("[UserController]-[UpdateUser]: Updating user with id {UserId}", id);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("[UserController]-[UpdateUser]: User with id {UserId} not found", id);
                return NotFound();
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Dob = request.Dob;
            user.Avatar = request.Avatar;
            user.IsActive = request.IsActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning("[UserController]-[UpdateUser]: Failed to update user with id {UserId}. Errors: {@Errors}", id, result.Errors);
                return BadRequest(result.Errors);
            }

            _logger.LogInformation("[UserController]-[UpdateUser]: Successfully updated user with id {UserId}", id);
            return Ok();
        }

        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword(Guid id, ChangeMyPasswordRequest request)
        {
            _logger.LogInformation("[UserController]-[ChangePassword]: Changing password for user with id {UserId}", id);
            var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
            //var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("[UserController]-[ChangePassword]: User with id {UserId} not found", id);
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("[UserController]-[ChangePassword]: Failed to change password for user with id {UserId}. Errors: {@Errors}", id, result.Errors);
                return BadRequest(result.Errors);
            }

            _logger.LogInformation("[UserController]-[ChangePassword]: Successfully changed password for user with id {UserId}", id);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Permissions.Users.Delete)]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            var users = await _userManager.Users
                .Where(w => ids.Contains(w.Id.ToString()))
                .ToListAsync();

            foreach (var itemUser in users)
            {
                var user = users.FirstOrDefault(f => f.Id.Equals(itemUser.Id));
                if (user == null)
                {
                    return NotFound();
                }
                await _userManager.DeleteAsync(user);
            }
            return Ok();
        }

        [HttpPost("set-password/{id}")]
        [Authorize(Permissions.Users.Edit)]
        public async Task<ActionResult> SetPassword(Guid id, [FromBody] SetPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.NewPassword);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpPost("change-email/{id}")]
        [Authorize(Permissions.Users.Edit)]
        public async Task<ActionResult> ChangeEmail(Guid id, [FromBody] ChangeEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.Email);
            var result = await _userManager.ChangeEmailAsync(user, request.Email, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }

        [HttpPut("{id}/assign-users")]
        [Authorize(Permissions.Users.Edit)]
        [ValidateModel]
        public async Task<IActionResult> AssignRolesToUser(string id, [FromBody] string[] roles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            var addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded || !removeResult.Succeeded)
            {
                List<IdentityError> addErrorList = addResult.Errors.ToList();
                List<IdentityError> removeErrorList = removeResult.Errors.ToList();
                var errorList = new List<IdentityError>();
                errorList.AddRange(addErrorList);
                errorList.AddRange(removeErrorList);
                return BadRequest(string.Join("</br>", errorList.Select(e => e.Description)));
            }
            return Ok();
        }
    }
}