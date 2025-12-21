using NUPAL.Core.Application.DTOs;

namespace NUPAL.Core.Application.Interfaces
{
    public interface IRlService
    {
        Task<RlTrainingResponse> GetRecommendationAsync(RlTrainingRequest request);
    }
}
