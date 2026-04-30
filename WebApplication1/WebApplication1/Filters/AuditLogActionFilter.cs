using MediPulses.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Filters;

public class AuditLogActionFilter : IAsyncActionFilter
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogActionFilter(IAuditLogService auditLogService) =>
        _auditLogService = auditLogService;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();

        // Only log successful POST requests (redirect = success; view return = validation failed)
        if (!context.HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            return;

        if (executed.Result is not (RedirectToActionResult or RedirectToRouteResult or LocalRedirectResult))
            return;

        var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
        var action     = context.RouteData.Values["action"]?.ToString() ?? "";

        // Skip controllers that shouldn't be logged
        if (controller is "AuditLog" or "Home" or "Account")
            return;

        var userId = context.HttpContext.Session.GetInt32("UserId");
        var description = BuildDescription(controller, action);

        try { await _auditLogService.RecordAsync(userId, description); }
        catch { /* audit failure must never break the main flow */ }
    }

    private static string BuildDescription(string controller, string action)
    {
        var entity = controller switch
        {
            "Facilities"         => "Facility",
            "Item"               => "Item",
            "Supplier"           => "Supplier",
            "StorageZones"       => "Storage Zone",
            "PurchaseOrder"      => "Purchase Order",
            "Receipt"            => "Receipt",
            "InventoryPosition"  => "Inventory Position",
            "ConsumptionRecord"  => "Consumption Record",
            "TransferOrder"      => "Transfer Order",
            "SensorDevice"       => "Sensor Device",
            "TelemetryRecord"    => "Telemetry Record",
            "Admin"              => "User",
            _                    => controller
        };

        var verb = action switch
        {
            "Create"  => "Created",
            "Edit"    => "Updated",
            "Delete"  => "Deleted",
            "Approve" => "Approved",
            _         => action
        };

        return $"{verb} {entity}";
    }
}
