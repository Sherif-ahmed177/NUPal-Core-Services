using NUPAL.Core.Application.DTOs;

namespace NUPAL.Core.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatSendResponseDto> SendAsync(string studentId, ChatSendRequestDto request, CancellationToken ct = default);
        Task<List<ChatConversationDto>> GetConversationsAsync(string studentId);
        Task<List<ChatMessageDto>> GetMessagesAsync(string conversationId);
        Task DeleteConversationAsync(string studentId, string conversationId);
        Task TogglePinAsync(string studentId, string conversationId, bool isPinned);
        Task RenameConversationAsync(string studentId, string conversationId, string newTitle);
    }
}
