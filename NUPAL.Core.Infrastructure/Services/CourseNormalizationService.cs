using NUPAL.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Nupal.Domain.Entities;

namespace Nupal.Core.Infrastructure.Services
{
    public class CourseNormalizationService : ICourseNormalizationService
    {
        private readonly ICourseMappingRepository _repository;
        private readonly ILogger<CourseNormalizationService> _logger;

        public CourseNormalizationService(ICourseMappingRepository repository, ILogger<CourseNormalizationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private List<CourseMapping>? _cachedMappings;

        public async Task<string> NormalizeToCodeAsync(string courseName)
        {
            if (string.IsNullOrWhiteSpace(courseName))
                return courseName;

            if (_cachedMappings == null)
            {
                _cachedMappings = await _repository.GetAllAsync();
            }

            var lowerName = courseName.Trim().ToLower();

            // First try by Code
            var byCode = _cachedMappings.FirstOrDefault(m => m.CourseCode != null && m.CourseCode.ToLower() == lowerName);
            if (byCode != null && !string.IsNullOrEmpty(byCode.CourseCode))
                return byCode.CourseCode;

            // Then try by Name
            var byName = _cachedMappings.FirstOrDefault(m => 
                (m.PolicyName != null && m.PolicyName.ToLower() == lowerName) ||
                (m.BlockNames != null && m.BlockNames.Any(b => b.ToLower() == lowerName)) ||
                (m.TrackNames != null && m.TrackNames.Any(t => t.ToLower() == lowerName)) ||
                (m.AcademicPlanNames != null && m.AcademicPlanNames.Any(a => a.ToLower() == lowerName))
            );

            if (byName != null && !string.IsNullOrEmpty(byName.CourseCode))
                return byName.CourseCode;

            _logger.LogWarning("Could not normalize course name: {CourseName}", courseName);
            return courseName;
        }

        public async Task<List<string>> NormalizeToCodesAsync(IEnumerable<string> courseNames)
        {
            // Pre-load cache once for the batch
            if (_cachedMappings == null)
            {
                _cachedMappings = await _repository.GetAllAsync();
            }

            var tasks = courseNames.Select(NormalizeToCodeAsync);
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }
    }
}
