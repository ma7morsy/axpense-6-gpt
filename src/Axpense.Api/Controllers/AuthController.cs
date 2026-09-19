using Axpense.Api.Data;
using Axpense.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Axpense.Api.Controllers;

[ApiController, Route("api/auth")]
public class AuthController(AxpenseDbContext db, IConfiguration config) : ControllerBase
{
    [AllowAnonymous, HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.OrganizationName) || string.IsNullOrWhiteSpace(r.FullName) ||
            string.IsNullOrWhiteSpace(r.Email) || string.IsNullOrWhiteSpace(r.Password) || r.Password.Length < 8)
            return BadRequest("Organization, name, email and a password of at least 8 characters are required.");
        var email = r.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.Email == email)) return Conflict("Email is already registered.");
        var org = new Organization { Name = r.OrganizationName.Trim() };
        var user = new User { OrganizationId = org.Id, FullName = r.FullName.Trim(), Email = email, PasswordHash = PasswordService.Hash(r.Password), Role = "Admin" };
        db.Organizations.Add(org); db.Users.Add(user); await db.SaveChangesAsync();
        return Ok(AuthResponse(user, org));
    }

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest r)
    {
        var email = r.Email.Trim().ToLowerInvariant();
        var user = await db.Users.Include(x => x.Organization).SingleOrDefaultAsync(x => x.Email == email);
        if (user is null || !user.IsActive || !PasswordService.Verify(r.Password, user.PasswordHash)) return Unauthorized("Invalid email or password.");
        return Ok(AuthResponse(user, user.Organization));
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var id)) return Unauthorized();
        var user = await db.Users.AsNoTracking().Include(x => x.Organization).SingleOrDefaultAsync(x => x.Id == id);
        return user is null ? Unauthorized() : Ok(new { user.Id, user.FullName, user.Email, user.Role, OrganizationId = user.OrganizationId, OrganizationName = user.Organization.Name });
    }

    private object AuthResponse(User u, Organization o) => new
    {
        token = JwtExtensions.CreateToken(config, u.Id, o.Id, u.Email, u.Role, u.FullName),
        user = new { u.Id, u.FullName, u.Email, u.Role, OrganizationId = o.Id, OrganizationName = o.Name }
    };
}

public record RegisterRequest(string OrganizationName, string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);
