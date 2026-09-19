using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using Axpense.Api.Data; using Axpense.Api.Domain;
namespace Axpense.Api.Controllers;
[ApiController][Route("api/drivers")]
public class DriversController(AxpenseDbContext db):ControllerBase {
 [HttpGet] public async Task<IActionResult> Get(Guid organizationId)=>Ok(await db.Drivers.Where(x=>x.OrganizationId==organizationId).OrderBy(x=>x.FullName).ToListAsync());
 [HttpPost] public async Task<IActionResult> Post(Driver d){if(d.OrganizationId==Guid.Empty||string.IsNullOrWhiteSpace(d.FullName))return BadRequest("Organization and full name are required."); d.Id=Guid.NewGuid();db.Drivers.Add(d);await db.SaveChangesAsync();return Created($"/api/drivers/{d.Id}",d);}
 [HttpPut("{id:guid}")] public async Task<IActionResult> Put(Guid id,Driver input){var d=await db.Drivers.FirstOrDefaultAsync(x=>x.Id==id&&x.OrganizationId==input.OrganizationId);if(d is null)return NotFound();d.FullName=input.FullName;d.Phone=input.Phone;d.LicenseNumber=input.LicenseNumber;d.LicenseExpiryDate=input.LicenseExpiryDate;d.Status=input.Status;d.HireDate=input.HireDate;d.Notes=input.Notes;await db.SaveChangesAsync();return Ok(d);}
 [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id,Guid organizationId){var d=await db.Drivers.FirstOrDefaultAsync(x=>x.Id==id&&x.OrganizationId==organizationId);if(d is null)return NotFound();db.Drivers.Remove(d);await db.SaveChangesAsync();return NoContent();}
}
