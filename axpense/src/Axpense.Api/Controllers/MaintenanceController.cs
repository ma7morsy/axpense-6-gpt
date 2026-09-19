using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axpense.Api.Data;
using Axpense.Api.Domain;
namespace Axpense.Api.Controllers;
[ApiController]
[Route("api/maintenance")]
public class MaintenanceController(AxpenseDbContext db) : ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get(Guid organizationId) => Ok(await db.MaintenanceRecords.AsNoTracking().Include(x=>x.Vehicle).Where(x=>x.OrganizationId==organizationId).OrderBy(x=>x.DueDate).Select(x=>new {x.Id,x.VehicleId,Vehicle=x.Vehicle.PlateNumber,x.Type,x.Status,x.Description,x.DueDate,x.EstimatedCost,x.ActualCost,x.OdometerAtService,x.CompletedAtUtc}).ToListAsync());
 [HttpPost] public async Task<IActionResult> Create(CreateMaintenanceRequest r){if(!await db.Vehicles.AnyAsync(x=>x.Id==r.VehicleId&&x.OrganizationId==r.OrganizationId))return BadRequest("Vehicle not found in this organization.");var x=new MaintenanceRecord{OrganizationId=r.OrganizationId,VehicleId=r.VehicleId,Type=r.Type??"Preventive",Status="Scheduled",Description=r.Description.Trim(),DueDate=r.DueDate,EstimatedCost=r.EstimatedCost};db.MaintenanceRecords.Add(x);await db.SaveChangesAsync();return Created($"/api/maintenance/{x.Id}",x);}
 [HttpPut("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id,CompleteRequest r){var x=await db.MaintenanceRecords.FindAsync(id);if(x is null)return NotFound();x.Status="Completed";x.ActualCost=r.ActualCost;x.CompletedAtUtc=DateTime.UtcNow;await db.SaveChangesAsync();return Ok(x);}
 [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id){var x=await db.MaintenanceRecords.FindAsync(id);if(x is null)return NotFound();db.MaintenanceRecords.Remove(x);await db.SaveChangesAsync();return NoContent();}
}
public record CreateMaintenanceRequest(Guid OrganizationId,Guid VehicleId,string Description,DateTime DueDate,decimal? EstimatedCost,string? Type);
public record CompleteRequest(decimal? ActualCost);
