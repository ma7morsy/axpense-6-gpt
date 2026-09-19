using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axpense.Api.Data;
using Axpense.Api.Domain;

namespace Axpense.Api.Controllers;

[ApiController, Route("api/saas")]
public class SaasController(AxpenseDbContext db) : ControllerBase
{
    Guid Tenant => Guid.Parse(User.FindFirstValue("organization_id")!);
    Guid? UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id) ? id : null;

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings(){ var s=await db.OrganizationSettings.FirstOrDefaultAsync(x=>x.OrganizationId==Tenant); return Ok(s ?? new OrganizationSettings{Id=Guid.NewGuid(),OrganizationId=Tenant,CompanyName=(await db.Organizations.FindAsync(Tenant))?.Name??""}); }

    [HttpPut("settings"), Authorize(Roles="Admin")]
    public async Task<IActionResult> SaveSettings(OrganizationSettings input){ var s=await db.OrganizationSettings.FirstOrDefaultAsync(x=>x.OrganizationId==Tenant); if(s==null){s=new OrganizationSettings{Id=Guid.NewGuid(),OrganizationId=Tenant};db.OrganizationSettings.Add(s);} s.Currency=input.Currency; s.TimeZone=input.TimeZone; s.DateFormat=input.DateFormat; s.CompanyName=input.CompanyName; s.LogoUrl=input.LogoUrl; s.EmailNotifications=input.EmailNotifications; s.MaintenanceReminders=input.MaintenanceReminders; s.LicenseReminders=input.LicenseReminders; await db.SaveChangesAsync(); await Audit("Updated organization settings","OrganizationSettings",s.Id.ToString()); return Ok(s); }

    [HttpGet("audit")]
    public async Task<IActionResult> AuditLogs([FromQuery]int take=100){take=Math.Clamp(take,1,500);return Ok(await db.AuditLogs.Where(x=>x.OrganizationId==Tenant).OrderByDescending(x=>x.CreatedAt).Take(take).ToListAsync());}

    [HttpPost("audit")]
    public async Task<IActionResult> AddAudit(AuditLog input){input.Id=Guid.NewGuid();input.OrganizationId=Tenant;input.UserId=UserId;input.CreatedAt=DateTime.UtcNow;db.AuditLogs.Add(input);await db.SaveChangesAsync();return Ok(input);}

    [HttpGet("export/vehicles.csv")]
    public async Task<IActionResult> ExportVehicles(){var rows=await db.Vehicles.Where(x=>x.OrganizationId==Tenant).Select(x=>new{x.PlateNumber,x.Make,x.Model,x.Year,x.Vin,x.Status,x.CurrentOdometer}).ToListAsync();var sb=new StringBuilder("PlateNumber,Make,Model,Year,VIN,Status,Odometer\n");foreach(var x in rows)sb.AppendLine($"{Esc(x.PlateNumber)},{Esc(x.Make)},{Esc(x.Model)},{x.Year},{Esc(x.Vin)},{Esc(x.Status)},{x.CurrentOdometer}");return File(Encoding.UTF8.GetBytes(sb.ToString()),"text/csv","axpense-vehicles.csv");}
    static string Esc(string? s)=>"\""+(s??"").Replace("\"","\"\"")+"\"";
    async Task Audit(string action,string entity,string? id){db.AuditLogs.Add(new AuditLog{Id=Guid.NewGuid(),OrganizationId=Tenant,UserId=UserId,Action=action,EntityType=entity,EntityId=id});await db.SaveChangesAsync();}
}
