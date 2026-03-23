using NUPAL.Core.Application.DTOs;
using Nupal.Domain.Entities;

namespace NUPAL.Core.Application.Interfaces
{
    public interface IJobFitService
    {
        Task<JobFitAnalysisDto> AnalyzeFitAsync(string? jobUrl, string? jobDescription, ResumeData resumeData, CancellationToken ct);
    }
}
