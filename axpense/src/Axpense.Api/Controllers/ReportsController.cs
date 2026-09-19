using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Axpense.Api.Data;
namespace Axpense.Api.Controllers;
[ApiController][Route("api/reports")]
public class ReportsController(AxpenseDbContext db):ControllerBase
{
 [HttpGet("summary")]
 public async Task<IActionResult> Summary(Guid organizationId, int? year, int? month)
 {
  var y=year??DateTime.UtcNow.Year; var m=month??DateTime.UtcNow.Month; var start=new DateTime(y,m,1); var end=start.AddMonths(1);
  var expenses=await db.Expenses.Where(x=>x.OrganizationId==organizationId&&x.ExpenseDate>=start&&x.ExpenseDate<end).SumAsync(x=>(decimal?)x.Amount)??0;
  var fuel=await db.FuelTransactions.Where(x=>x.OrganizationId==organizationId&&x.TransactionDate>=start&&x.TransactionDate<end).SumAsync(x=>(decimal?)x.TotalAmount)??0;
  var maintenance=await db.MaintenanceRecords.Where(x=>x.OrganizationId==organizationId&&x.CompletedAtUtc>=start&&x.CompletedAtUtc<end).SumAsync(x=>(decimal?)x.ActualCost)??0;
  return Ok(new {year=y,month=m,expenses,fuel,maintenance,total=expenses+fuel+maintenance});
 }
 [HttpGet("by-vehicle")]
 public async Task<IActionResult> ByVehicle(Guid organizationId, int? year, int? month)
 {
  var y=year??DateTime.UtcNow.Year; var m=month??DateTime.UtcNow.Month; var start=new DateTime(y,m,1); var end=start.AddMonths(1);
  var vehicles=await db.Vehicles.Where(v=>v.OrganizationId==organizationId).Select(v=>new {v.Id,v.PlateNumber,v.Make,v.Model}).ToListAsync();
  var expenses=await db.Expenses.Where(x=>x.OrganizationId==organizationId&&x.VehicleId!=null&&x.ExpenseDate>=start&&x.ExpenseDate<end).GroupBy(x=>x.VehicleId!.Value).Select(g=>new {VehicleId=g.Key,Amount=g.Sum(x=>x.Amount)}).ToListAsync();
  var fuel=await db.FuelTransactions.Where(x=>x.OrganizationId==organizationId&&x.TransactionDate>=start&&x.TransactionDate<end).GroupBy(x=>x.VehicleId).Select(g=>new {VehicleId=g.Key,Amount=g.Sum(x=>x.TotalAmount)}).ToListAsync();
  var maintenance=await db.MaintenanceRecords.Where(x=>x.OrganizationId==organizationId&&x.CompletedAtUtc>=start&&x.CompletedAtUtc<end&&x.ActualCost!=null).GroupBy(x=>x.VehicleId).Select(g=>new {VehicleId=g.Key,Amount=g.Sum(x=>x.ActualCost!.Value)}).ToListAsync();
  var result=vehicles.Select(v=>{var e=expenses.FirstOrDefault(x=>x.VehicleId==v.Id)?.Amount??0;var f=fuel.FirstOrDefault(x=>x.VehicleId==v.Id)?.Amount??0;var mt=maintenance.FirstOrDefault(x=>x.VehicleId==v.Id)?.Amount??0;return new {v.Id,vehicle=$"{v.PlateNumber} · {v.Make} {v.Model}",expenses=e,fuel=f,maintenance=mt,total=e+f+mt};}).OrderByDescending(x=>x.total);
  return Ok(result);
 }
 [HttpGet("by-category")]
 public async Task<IActionResult> ByCategory(Guid organizationId, int? year, int? month)
 {
  var y=year??DateTime.UtcNow.Year; var m=month??DateTime.UtcNow.Month; var start=new DateTime(y,m,1); var end=start.AddMonths(1);
  var result=await db.Expenses.Where(x=>x.OrganizationId==organizationId&&x.ExpenseDate>=start&&x.ExpenseDate<end).GroupBy(x=>x.Category).Select(g=>new {category=g.Key,amount=g.Sum(x=>x.Amount),count=g.Count()}).OrderByDescending(x=>x.amount).ToListAsync();
  return Ok(result);
 }
}
