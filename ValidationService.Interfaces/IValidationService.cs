namespace ValidationService.Interfaces
{
    using Contracts;
    using SnurfReportService.Interfaces;
    using Microsoft.ServiceFabric.Services.Remoting;
    using System.Threading.Tasks;

    public interface ISnurfReportValidationService : IService
    {
        Task<ValidationResult> ValidateSnurfReport(ReportBase report);
        Task<ValidationResult> ValidateSurfReport(SurfReport report);
        Task<ValidationResult> ValidateSnowReport(SnowReport report);
    }
}
