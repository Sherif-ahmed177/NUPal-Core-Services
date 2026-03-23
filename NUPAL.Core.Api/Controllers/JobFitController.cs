using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUPAL.Core.Application.Interfaces;
using NUPAL.Core.Application.DTOs;

namespace NUPAL.Core.API.Controllers
{
    [ApiController]
    [Route("api/resume/job-fit")]
    [Authorize]
    public class JobFitController : ControllerBase
    {
        private readonly IJobFitService _jobFitService;
        private readonly IResumeRepository _resumeRepository;
        private readonly ILogger<JobFitController> _logger;

        public JobFitController(
            IJobFitService jobFitService,
            IResumeRepository resumeRepository,
            ILogger<JobFitController> logger)
        {
            _jobFitService = jobFitService;
            _resumeRepository = resumeRepository;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeFit([FromBody] AnalyzeFitRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.JobUrl))
                return BadRequest(new { error = "Job URL is required." });

            try
            {
                // 1. Get Resume Data
                var email = User.Identity?.Name;
                if (string.IsNullOrEmpty(email)) return Unauthorized();

                // If no resumeId provided, fetch the latest one
                var history = await _resumeRepository.GetByStudentEmailAsync(email);
                var latest = string.IsNullOrEmpty(request.ResumeId) 
                    ? history.FirstOrDefault() 
                    : history.FirstOrDefault(h => h.Id.ToString() == request.ResumeId);

                if (latest == null)
                    return BadRequest(new { error = "No resume found for analysis. Please upload your resume first." });

                // 2. Perform Analysis
                var analysis = await _jobFitService.AnalyzeFitAsync(request.JobUrl, latest.Data, ct);
                
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Job Fit Analysis");
                return StatusCode(500, new { error = "Fit analysis failed", message = ex.Message });
            }
        }
    }

    public class AnalyzeFitRequest
    {
        public string JobUrl { get; set; } = string.Empty;
        public string? ResumeId { get; set; }
    }
}
