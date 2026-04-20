using AutoServiceApp.ViewModels;

namespace AutoServiceApp.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetStatsAsync();
}
