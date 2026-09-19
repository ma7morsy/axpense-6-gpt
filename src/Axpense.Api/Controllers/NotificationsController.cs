using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using Axpense.Api.Data; using Axpense.Api.Domain;
namespace Axpense.Api.Controllers;
[ApiController][Route("api/notifications")]
public class NotificationsController(AxpenseDbContext db):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get(Guid organizationId,bool unreadOnly=false){var q=db.Notifications.Where(x=>x.OrganizationId==organizationId);if(unreadOnly)q=q.Where(x=>!x.IsRead);return Ok(await q.OrderByDescending(x=>x.CreatedAt).Take(100).ToListAsync());}
 [HttpPost] public async Task<IActionResult> Create(Notification n){n.Id=Guid.NewGuid();n.CreatedAt=DateTime.UtcNow;db.Notifications.Add(n);await db.SaveChangesAsync();return Ok(n);}
 [HttpPost("{id}/read")] public async Task<IActionResult> Read(Guid id){var n=await db.Notifications.FindAsync(id);if(n is null)return NotFound();n.IsRead=true;await db.SaveChangesAsync();return Ok(n);}
 [HttpPost("generate")]
 public async Task<IActionResult> Generate(Guid organizationId){var now=DateTime.UtcNow.Date;var until=now.AddDays(30);var created=0;
  var maint=await db.MaintenanceRecords.Where(x=>x.OrganizationId==organizationId && x.CompletedAtUtc==null && x.DueDate<=until).ToListAsync();
  foreach(var m in maint){if(!await db.Notifications.AnyAsync(n=>n.OrganizationId==organizationId&&n.Type=="Maintenance"&&n.DueDate==m.DueDate&&n.Message.Contains(m.VehicleId.ToString()))){db.Notifications.Add(new Notification{Id=Guid.NewGuid(),OrganizationId=organizationId,Type="Maintenance",Title="Maintenance due",Message=$"Vehicle {m.VehicleId} has maintenance due on {m.DueDate:yyyy-MM-dd}. Reference: {m.VehicleId}",Severity=m.DueDate<now?"Critical":"Warning",DueDate=m.DueDate});created++;}}
  var drivers=await db.Drivers.Where(x=>x.OrganizationId==organizationId&&x.LicenseExpiryDate!=null&&x.LicenseExpiryDate<=until).ToListAsync();
  foreach(var d in drivers){if(!await db.Notifications.AnyAsync(n=>n.OrganizationId==organizationId&&n.Type=="License"&&n.Message.Contains(d.Id.ToString()))){db.Notifications.Add(new Notification{Id=Guid.NewGuid(),OrganizationId=organizationId,Type="License",Title="Driver license expiring",Message=$"Driver {d.FullName} has a license expiring on {d.LicenseExpiryDate:yyyy-MM-dd}. Reference: {d.Id}",Severity=d.LicenseExpiryDate<now?"Critical":"Warning",DueDate=d.LicenseExpiryDate});created++;}}
  await db.SaveChangesAsync();return Ok(new{created}); }
}
