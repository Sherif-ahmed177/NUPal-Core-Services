using NUPAL.Core.Application.DTOs;

namespace NUPAL.Core.Application.Interfaces
{
    public interface IAgentClient
    {
        Task<AgentRouteResponseDto> RouteAsync(AgentRouteRequestDto request, CancellationToken ct = default);
        Task<AgentTitleResponseDto?> GenerateConversationTitleAsync(AgentTitleRequestDto request, CancellationToken ct = default);
    }
}
