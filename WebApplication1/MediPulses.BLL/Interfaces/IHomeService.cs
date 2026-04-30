using MediPulses.BLL.DTOs;

namespace MediPulses.BLL.Interfaces;

public interface IHomeService
{
    Task<DashboardSummary> GetDashboardSummaryAsync();
}
