namespace Axpense.Api.Domain;
public class Inspection : TenantEntity
{
    public Guid VehicleId { get; set; }
    public DateTime InspectionDate { get; set; } = DateTime.UtcNow;
    public string Type { get; set; } = "Routine";
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? InspectorUserId { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public ICollection<InspectionItem> Items { get; set; } = [];
}
public class InspectionItem : TenantEntity
{
    public Guid InspectionId { get; set; }
    public string ChecklistItem { get; set; } = "";
    public bool Passed { get; set; }
    public string? FailureReason { get; set; }
    public string? ImageUrl { get; set; }
    public Inspection Inspection { get; set; } = null!;
}
