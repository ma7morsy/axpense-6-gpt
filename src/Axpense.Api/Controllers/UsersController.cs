using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using Axpense.Api.Data; using Axpense.Api.Domain;
using Axpense.Api;
namespace Axpense.Api.Controllers;
[ApiController][Route("api/users")]
public class UsersController(AxpenseDbContext db):ControllerBase {
 Guid Tenant=>Guid.Parse(User.FindFirstValue("organization_id")!);
 [HttpGet] public async Task<IActionResult> Get()=>Ok(await db.Users.AsNoTracking().Where(x=>x.OrganizationId==Tenant).Select(x=>new{x.Id,x.FullName,x.Email,x.Role,x.IsActive,x.CreatedAtUtc}).ToListAsync());
 [Authorize(Roles="Admin")][HttpPost] public async Task<IActionResult> Create(CreateUserRequest r){if(r.OrganizationId!=Tenant)return Forbid();if(string.IsNullOrWhiteSpace(r.Password))return BadRequest("Password is required.");if(await db.Users.AnyAsync(x=>x.OrganizationId==Tenant&&x.Email==r.Email.Trim().ToLowerInvariant()))return Conflict("Email already exists in this organization.");var u=new User{OrganizationId=Tenant,FullName=r.FullName.Trim(),Email=r.Email.Trim().ToLowerInvariant(),PasswordHash=PasswordService.Hash(r.Password),Role=string.IsNullOrWhiteSpace(r.Role)?"User":r.Role,IsActive=true};db.Users.Add(u);await db.SaveChangesAsync();return Created($"/api/users/{u.Id}",new{u.Id,u.FullName,u.Email,u.Role,u.IsActive});}
 [Authorize(Roles="Admin")][HttpPut("{id}/status")] public async Task<IActionResult> Status(Guid id,[FromBody] UserStatusRequest r){var u=await db.Users.FirstOrDefaultAsync(x=>x.Id==id&&x.OrganizationId==Tenant);if(u==null)return NotFound();u.IsActive=r.IsActive;await db.SaveChangesAsync();return Ok(new{u.Id,u.IsActive});}
}
public record CreateUserRequest(Guid OrganizationId,string FullName,string Email,string Password,string? Role);
public record UserStatusRequest(bool IsActive);
