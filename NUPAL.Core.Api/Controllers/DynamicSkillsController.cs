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

        public DynamicSkillsController(ILogger<DynamicSkillsController> logger, IStudentRepository studentRepository)
        {
            _logger = logger;
            _studentRepository = studentRepository;
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

                // Extract and calculate skills from courses
                var skills = ExtractSkillsFromCourses(student);

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

        private List<object> ExtractSkillsFromCourses(Student student)
        {
            var skillMap = new Dictionary<string, List<double>>();

            try
            {
                // Iterate over domain entities directly
                if (student.Education?.Semesters != null)
                {
                    foreach (var semester in student.Education.Semesters)
                    {
                        if (semester.Courses != null)
                        {
                            foreach (var course in semester.Courses)
                            {
                                var courseName = course.CourseName?.ToLower() ?? "";
                                _logger.LogInformation("Processing course: {CourseName}", courseName); // Debug log
                                var grade = course.Grade ?? "";
                                var proficiency = GradeToProficiency(grade);

                                // Map courses to skills
                                if (courseName.Contains("programming") || courseName.Contains("python"))
                                {
                                    AddSkill(skillMap, "Python", proficiency);
                                }
                                if (courseName.Contains("data structures") || courseName.Contains("algorithms"))
                                {
                                    AddSkill(skillMap, "Data Structures", proficiency);
                                }
                                if (courseName.Contains("machine learning") || courseName.Contains("ai"))
                                {
                                    AddSkill(skillMap, "Machine Learning", proficiency);
                                }
                                if (courseName.Contains("web") || courseName.Contains("internet"))
                                {
                                    AddSkill(skillMap, "Web Development", proficiency);
                                }
                                if (courseName.Contains("database") || courseName.Contains("data mining"))
                                {
                                    AddSkill(skillMap, "Databases", proficiency);
                                    AddSkill(skillMap, "SQL", proficiency); // Added SQL from Database
                                }
                                if (courseName.Contains("network") || courseName.Contains("security"))
                                {
                                    AddSkill(skillMap, "Networking", proficiency);
                                }
                                if (courseName.Contains("software")) // Added Git from Software Engineering
                                {
                                    AddSkill(skillMap, "Git", proficiency);
                                    AddSkill(skillMap, "Software Engineering", proficiency);
                                }
                                if (courseName.Contains("linear")) // Added Linear Algebra
                                {
                                    AddSkill(skillMap, "Linear Algebra", proficiency);
                                }
                                if (courseName.Contains("big data")) // Added Docker from Big Data
                                {
                                    AddSkill(skillMap, "Docker", proficiency);
                                    AddSkill(skillMap, "Big Data", proficiency);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting skills from courses");
            }

            // Calculate average proficiency for each skill
            var skills = new List<object>();
            foreach (var skill in skillMap)
            {
                var avgProficiency = (int)Math.Round(skill.Value.Average());
                skills.Add(new
                {
                    name = skill.Key,
                    level = avgProficiency,
                    category = GetSkillCategory(skill.Key)
                });
            }

            return skills.OrderByDescending(s => ((dynamic)s).level).ToList();
        }

        private void AddSkill(Dictionary<string, List<double>> skillMap, string skillName, double proficiency)
        {
            if (!skillMap.ContainsKey(skillName))
            {
                skillMap[skillName] = new List<double>();
            }
            skillMap[skillName].Add(proficiency);
        }

        private double GradeToProficiency(string grade)
        {
            return grade switch
            {
                "A+" => 98,
                "A" => 93,
                "B+" => 87,
                "B" => 83,
                "C+" => 77,
                "C" => 73,
                "D+" => 67,
                "D" => 63,
                "P" => 80, // Pass implies good proficiency
                _ => 70 // Default
            };
        }

        private string GetSkillCategory(string skillName)
        {
            return skillName switch
            {
                "Python" => "Programming",
                "Data Structures" => "Computer Science",
                "Machine Learning" => "AI/ML",
                "Web Development" => "Development",
                "Databases" => "Data Management",
                "SQL" => "Data Management",
                "Networking" => "Infrastructure",
                "Git" => "DevOps",
                "Software Engineering" => "Methodology",
                "Linear Algebra" => "Mathematics",
                "Docker" => "DevOps",
                "Big Data" => "Data Engineering",
                _ => "General"
            };
        }
    }
}
