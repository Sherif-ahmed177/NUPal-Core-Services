using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NUPAL.Core.Application.Interfaces;
using Nupal.Domain.Entities;

namespace NUPAL.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DynamicSkillsController : ControllerBase
    {
        private readonly ILogger<DynamicSkillsController> _logger;
        private readonly IStudentRepository _studentRepository;
        private readonly IDynamicSkillsService _dynamicSkillsService;

        public DynamicSkillsController(
            ILogger<DynamicSkillsController> logger, 
            IStudentRepository studentRepository,
            IDynamicSkillsService dynamicSkillsService)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _dynamicSkillsService = dynamicSkillsService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Get student ID from JWT token
                var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(studentId))
                {
                    return Unauthorized(new { message = "Student ID not found in token" });
                }

                // Fetch student data from database repository
                var student = await _studentRepository.GetByIdAsync(studentId);
                
                if (student == null)
                {
                    _logger.LogWarning("Student not found in database: {StudentId}", studentId);
                    return NotFound(new { message = "Student data not found" });
                }

                // Extract student name
                var name = student.Account?.Name ?? "Student";

                // Extract and calculate skills from courses using service
                var skills = _dynamicSkillsService.ExtractSkillsFromCourses(student);

                return Ok(new
                {
                    name,
                    skills
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching student profile");
                return StatusCode(500, new { message = "Error loading student profile" });
            }
        }
    }
}
