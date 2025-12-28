using NUPAL.Core.Application.DTOs;

namespace NUPAL.Core.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatSendResponseDto> SendAsync(string studentId, ChatSendRequestDto request, CancellationToken ct = default);
    }
}
