using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Axpense.Api;
using Axpense.Api.Data;
using Axpense.Api.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddResponseCompression();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("api", o =>
    {
        o.PermitLimit = 120;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueLimit = 0;
        o.AutoReplenishment = true;
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AxpenseDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks().AddDbContextCheck<AxpenseDbContext>();
builder.Services.AddAxpenseAuthentication(builder.Configuration);
var app = builder.Build();
app.UseExceptionHandler();
app.UseResponseCompression();
app.UseRateLimiter();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AxpenseDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Users.AnyAsync())
    {
        var org = new Organization { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Axpense Demo Organization" };
        var user = new User { OrganizationId = org.Id, FullName = "Axpense Admin", Email = "admin@axpense.local", PasswordHash = PasswordService.Hash("Axpense123!"), Role = "Admin" };
        db.Organizations.Add(org); db.Users.Add(user); await db.SaveChangesAsync();
    }
}
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<TenantAuthorizationMiddleware>();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("api");
app.MapHealthChecks("/health");
app.MapGet("/ready", async (AxpenseDbContext db, CancellationToken ct) =>
{
    var ready = await db.Database.CanConnectAsync(ct);
    return ready ? Results.Ok(new { status = "ready" }) : Results.StatusCode(503);
});
app.Run();
