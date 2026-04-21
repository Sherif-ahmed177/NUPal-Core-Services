using NUPAL.Core.Application.DTOs;

namespace NUPAL.Core.Application.Interfaces
{
    public interface ISchedulingService
    {
       
        Task<IEnumerable<RawBlockDto>> GetBlocksAsync(string? level = null);

        Task<RawBlockDto?> GetBlockAsync(string blockId);
        Task<IEnumerable<string>> GetCourseNamesAsync(string? level = null);

        Task<IEnumerable<string>> GetInstructorsAsync(string? level = null);
        Task<IEnumerable<RecommendationResultDto>> RecommendAsync(RecommendationRequestDto request);

        Task<int> SeedBlocksAsync(IEnumerable<RawBlockDto> blocks);
    }
}
