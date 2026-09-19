using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using Axpense.Api.Data; using Axpense.Api.Domain;
namespace Axpense.Api.Controllers;
[ApiController][Route("api/expenses")]
public class ExpensesController(AxpenseDbContext db):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get(Guid organizationId, DateTime? from=null, DateTime? to=null){var q=db.Expenses.AsNoTracking().Where(x=>x.OrganizationId==organizationId);if(from.HasValue)q=q.Where(x=>x.ExpenseDate>=from);if(to.HasValue)q=q.Where(x=>x.ExpenseDate<=to);return Ok(await q.OrderByDescending(x=>x.ExpenseDate).ToListAsync());}
 [HttpPost] public async Task<IActionResult> Post(Expense x){if(x.OrganizationId==Guid.Empty)return BadRequest("OrganizationId is required.");if(x.Amount<=0)return BadRequest("Amount must be greater than zero.");if(x.VehicleId.HasValue&&!await db.Vehicles.AnyAsync(v=>v.Id==x.VehicleId&&v.OrganizationId==x.OrganizationId))return BadRequest("Vehicle does not belong to the organization.");db.Expenses.Add(x);await db.SaveChangesAsync();return Created($"/api/expenses/{x.Id}",x);}
 [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id,Guid organizationId){var x=await db.Expenses.FirstOrDefaultAsync(e=>e.Id==id&&e.OrganizationId==organizationId);if(x is null)return NotFound();db.Remove(x);await db.SaveChangesAsync();return NoContent();}
}
