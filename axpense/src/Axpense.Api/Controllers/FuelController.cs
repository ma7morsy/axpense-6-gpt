using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using Axpense.Api.Data; using Axpense.Api.Domain;
namespace Axpense.Api.Controllers;
[ApiController][Route("api/fuel")]
public class FuelController(AxpenseDbContext db):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get(Guid organizationId){return Ok(await db.FuelTransactions.AsNoTracking().Where(x=>x.OrganizationId==organizationId).OrderByDescending(x=>x.TransactionDate).ToListAsync());}
 [HttpPost] public async Task<IActionResult> Post(FuelTransaction x){if(x.OrganizationId==Guid.Empty)return BadRequest("OrganizationId is required.");if(x.QuantityLiters<=0||x.UnitPrice<=0)return BadRequest("Quantity and unit price must be greater than zero.");if(!await db.Vehicles.AnyAsync(v=>v.Id==x.VehicleId&&v.OrganizationId==x.OrganizationId))return BadRequest("Vehicle does not belong to the organization.");x.TotalAmount=x.QuantityLiters*x.UnitPrice;db.FuelTransactions.Add(x);await db.SaveChangesAsync();return Created($"/api/fuel/{x.Id}",x);}
}
