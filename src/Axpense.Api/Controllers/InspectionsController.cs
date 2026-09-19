using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axpense.Api.Data;
using Axpense.Api.Domain;
namespace Axpense.Api.Controllers;
[ApiController][Route("api/inspections")]
public class InspectionsController(AxpenseDbContext db):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get(Guid organizationId){return Ok(await db.Inspections.AsNoTracking().Include(x=>x.Vehicle).Include(x=>x.Items).Where(x=>x.OrganizationId==organizationId).OrderByDescending(x=>x.InspectionDate).Select(x=>new{x.Id,x.VehicleId,Vehicle=x.Vehicle.PlateNumber,x.InspectionDate,x.Type,x.Status,x.Notes,x.ImageUrl,x.CompletedAtUtc,Items=x.Items.Select(i=>new{i.Id,i.ChecklistItem,i.Passed,i.FailureReason,i.ImageUrl})}).ToListAsync());}
 [HttpPost] public async Task<IActionResult> Create(CreateInspectionRequest r){if(!await db.Vehicles.AnyAsync(x=>x.Id==r.VehicleId&&x.OrganizationId==r.OrganizationId))return BadRequest("Vehicle not found in this organization.");var x=new Inspection{OrganizationId=r.OrganizationId,VehicleId=r.VehicleId,InspectionDate=r.InspectionDate,Type=r.Type??"Routine",Notes=r.Notes,ImageUrl=r.ImageUrl};foreach(var i in r.Items??[])x.Items.Add(new InspectionItem{OrganizationId=r.OrganizationId,ChecklistItem=i.ChecklistItem,Passed=i.Passed,FailureReason=i.Passed?null:i.FailureReason,ImageUrl=i.ImageUrl});db.Inspections.Add(x);await db.SaveChangesAsync();return Created($"/api/inspections/{x.Id}",x);}
 [HttpPut("{id:guid}/complete")] public async Task<IActionResult> Complete(Guid id){var x=await db.Inspections.Include(i=>i.Items).FirstOrDefaultAsync(i=>i.Id==id);if(x is null)return NotFound();x.Status=x.Items.Any(i=>!i.Passed)?"Failed":"Passed";x.CompletedAtUtc=DateTime.UtcNow;await db.SaveChangesAsync();return Ok(x);}
 [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id){var x=await db.Inspections.FindAsync(id);if(x is null)return NotFound();db.Inspections.Remove(x);await db.SaveChangesAsync();return NoContent();}
}
public record InspectionItemRequest(string ChecklistItem,bool Passed,string? FailureReason,string? ImageUrl);
public record CreateInspectionRequest(Guid OrganizationId,Guid VehicleId,DateTime InspectionDate,string? Type,string? Notes,string? ImageUrl,List<InspectionItemRequest>? Items);
