namespace Axpense.Api.Domain;
public class MaintenanceRecord : TenantEntity
{
    public Guid VehicleId { get; set; }
    public string Type { get; set; } = "Preventive";
    public string Status { get; set; } = "Scheduled";
    public string Description { get; set; } = "";
    public DateTime DueDate { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public decimal? OdometerAtService { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
}
