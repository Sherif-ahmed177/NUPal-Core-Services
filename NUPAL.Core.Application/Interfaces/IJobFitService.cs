using NUPAL.Core.Application.DTOs;
using Nupal.Domain.Entities;

namespace NUPAL.Core.Application.Interfaces
{
    public interface IJobFitService
    {
        Task<JobFitAnalysisDto> AnalyzeFitAsync(string jobUrl, ResumeData resumeData, CancellationToken ct);
    }
}
