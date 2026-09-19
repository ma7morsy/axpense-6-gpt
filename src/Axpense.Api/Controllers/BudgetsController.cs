using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using Axpense.Api.Data; using Axpense.Api.Domain;
namespace Axpense.Api.Controllers;
[ApiController][Route("api/budgets")]
public class BudgetsController(AxpenseDbContext db):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get(Guid organizationId,int? year=null,int? month=null){var q=db.Budgets.AsNoTracking().Where(x=>x.OrganizationId==organizationId);if(year.HasValue)q=q.Where(x=>x.Year==year);if(month.HasValue)q=q.Where(x=>x.Month==month);return Ok(await q.OrderByDescending(x=>x.Year).ThenByDescending(x=>x.Month).ToListAsync());}
 [HttpPost] public async Task<IActionResult> Post(Budget x){if(x.OrganizationId==Guid.Empty)return BadRequest("OrganizationId is required.");if(x.LimitAmount<=0||x.Month<1||x.Month>12)return BadRequest("Invalid budget period or amount.");db.Budgets.Add(x);await db.SaveChangesAsync();return Created($"/api/budgets/{x.Id}",x);}
}
