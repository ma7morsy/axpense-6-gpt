namespace Axpense.Api.Domain;

public class Expense : TenantEntity
{
    public Guid? VehicleId { get; set; }
    public string Category { get; set; } = "Other";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow.Date;
    public string? Vendor { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? Notes { get; set; }
    public Vehicle? Vehicle { get; set; }
}

public class FuelTransaction : TenantEntity
{
    public Guid VehicleId { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow.Date;
    public decimal QuantityLiters { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? Odometer { get; set; }
    public string? FuelType { get; set; }
    public string? Station { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
}

public class Budget : TenantEntity
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "Overall";
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal LimitAmount { get; set; }
    public string? Notes { get; set; }
}
