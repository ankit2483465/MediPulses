using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = null!;

    public string? Role { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(10, ErrorMessage = "Phone number is too long")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number must contain only digits")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters and contain uppercase, lowercase, number, and special character.")]
    public string Password { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public virtual ICollection<ConsumptionRecord> ConsumptionRecords { get; set; } = new List<ConsumptionRecord>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<RecallAction> RecallActions { get; set; } = new List<RecallAction>();
    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}