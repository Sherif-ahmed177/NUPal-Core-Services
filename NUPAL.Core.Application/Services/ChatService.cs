using System.Text.Json;
using Nupal.Domain.Entities;
using NUPAL.Core.Application.DTOs;
using NUPAL.Core.Application.Interfaces;

namespace NUPAL.Core.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatConversationRepository _convoRepo;
        private readonly IChatMessageRepository _msgRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IRlRecommendationRepository _rlRepo;
        private readonly IAgentClient _agent;

        public ChatService(
            IChatConversationRepository convoRepo,
            IChatMessageRepository msgRepo,
            IStudentRepository studentRepo,
            IRlRecommendationRepository rlRepo,
            IAgentClient agent)
        {
            _convoRepo = convoRepo;
            _msgRepo = msgRepo;
            _studentRepo = studentRepo;
            _rlRepo = rlRepo;
            _agent = agent;
        }

        public async Task<ChatSendResponseDto> SendAsync(string studentId, ChatSendRequestDto request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new ArgumentException("studentId is required");

            if (request == null || string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("message is required");

            // 1) Resolve conversation
            ChatConversation? convo = null;
            if (!string.IsNullOrWhiteSpace(request.ConversationId))
            {
                convo = await _convoRepo.GetByIdAsync(request.ConversationId);
                if (convo != null && convo.StudentId != studentId)
                {
                    // Prevent cross-user access
                    convo = null;
                }
            }

            if (convo == null)
            {
                convo = await _convoRepo.CreateAsync(new ChatConversation
                {
                    StudentId = studentId,
                    CreatedAt = DateTime.UtcNow,
                    LastActivityAt = DateTime.UtcNow
                });
            }

            var convoId = convo.Id.ToString();

            // 2) Fetch history (oldest -> newest)
            var history = await _msgRepo.GetRecentByConversationAsync(convoId, limit: 30);
            var agentHistory = history
                .OrderBy(m => m.CreatedAt)
                .Select(m => new AgentHistoryMessageDto
                {
                    Role = m.Role,
                    Kind = m.Kind,
                    Content = m.Content
                })
                .ToList();

            // Add current message for routing (not yet persisted so we can tag it correctly)
            agentHistory.Add(new AgentHistoryMessageDto
            {
                Role = "user",
                Kind = "unknown",
                Content = request.Message.Trim()
            });

            // 4) Fetch latest RL recommendation snapshot (if present)
            AgentRlRecommendationDto? rlSnap = null;
            var student = await _studentRepo.GetByIdAsync(studentId);
            if (student != null)
            {
                RlRecommendation? rl = null;
                if (!string.IsNullOrWhiteSpace(student.LatestRecommendationId))
                {
                    rl = await _rlRepo.GetByIdAsync(student.LatestRecommendationId);
                    Console.WriteLine($"[ChatService] Found RL via LatestRecommendationId: {student.LatestRecommendationId}");
                }
                rl ??= await _rlRepo.GetLatestByStudentIdAsync(studentId);

                if (rl != null)
                {
                    rlSnap = new AgentRlRecommendationDto
                    {
                        TermIndex = rl.TermIndex,
                        Courses = rl.Courses ?? new List<string>(),
                        SlatesByTerm = rl.SlatesByTerm,
                        Metrics = rl.Metrics,
                        ModelVersion = rl.ModelVersion,
                        PolicyVersion = rl.PolicyVersion
                    };
                    Console.WriteLine($"[ChatService] Sending RL recommendation to agent: TermIndex={rl.TermIndex}, Courses={rl.Courses?.Count ?? 0}");
                }
                else
                {
                    Console.WriteLine($"[ChatService] No RL recommendation found for student: {studentId}");
                }
            }
            else
            {
                Console.WriteLine($"[ChatService] Student not found: {studentId}");
            }

            // 5) Route via agent
            var agentReq = new AgentRouteRequestDto
            {
                StudentId = studentId,
                Message = request.Message.Trim(),
                History = agentHistory,
                RlRecommendation = rlSnap
            };

            var agentResp = await _agent.RouteAsync(agentReq, ct);

            // 6) Persist the user message with the resolved kind
            var replyKinds = agentResp.Results.Select(r => r.Kind).Distinct().ToList();
            var userKind = replyKinds.Count switch
            {
                0 => agentResp.Intent == "recommendation" ? "rl" : "rag",
                1 => replyKinds[0],
                _ => "mixed"
            };

            await _msgRepo.CreateAsync(new ChatMessage
            {
                ConversationId = convoId,
                StudentId = studentId,
                Role = "user",
                Kind = userKind,
                Content = request.Message.Trim(),
                CreatedAt = DateTime.UtcNow
            });

            // 7) Persist assistant replies
            var replies = new List<ChatReplyDto>();
            foreach (var r in agentResp.Results)
            {
                var metadataJson = r.Metadata == null ? null : JsonSerializer.Serialize(r.Metadata);

                await _msgRepo.CreateAsync(new ChatMessage
                {
                    ConversationId = convoId,
                    StudentId = studentId,
                    Role = "assistant",
                    Kind = r.Kind,
                    Content = r.Answer,
                    MetadataJson = metadataJson,
                    CreatedAt = DateTime.UtcNow
                });

                replies.Add(new ChatReplyDto
                {
                    Kind = r.Kind,
                    Content = r.Answer,
                    MetadataJson = metadataJson
                });
            }

            await _convoRepo.TouchAsync(convoId);

            // Update title if it's a new conversation (or has no title) and this is the first user message
            if (string.IsNullOrEmpty(convo.Title))
            {
               // Simple strategy: use the first 50 chars of the message
               var title = request.Message.Trim();
               if (title.Length > 50) title = title.Substring(0, 50) + "...";
               convo.Title = title;
               await _convoRepo.UpdateAsync(convo); 
            }

            return new ChatSendResponseDto
            {
                ConversationId = convoId,
                Replies = replies
            };
        }

        public async Task<List<ChatConversationDto>> GetConversationsAsync(string studentId)
        {
            var convos = await _convoRepo.GetLatestByStudentAsync(studentId);
            var results = new List<ChatConversationDto>();

            foreach (var c in convos)
            {
                // If title is missing, try to resolve it from the first message
                if (string.IsNullOrEmpty(c.Title))
                {
                    // Fetch oldest message because title usually comes from start
                    // But our repo only has "GetRecent" which is sorted desc. 
                    // Let's get "Recent" limit 1? No, recent is newest.
                    // We need a message to be the title. Newer is arguably better than nothing?
                    // Let's fetch recent messages.
                    var msgs = await _msgRepo.GetRecentByConversationAsync(c.Id.ToString(), 1);
                    
                    if (msgs.Any())
                    {
                        var firstMsg = msgs.First(); 
                        // Wait, if we want the "original" request, we might want the oldest. But we don't have GetOldest.
                        // For lazy migration purposes, the Last message is fine, or any message is fine.
                        // Actually, if we just want to filter EMPTY chats, checking Any() is enough.
                        
                        // We can set a generic title or use the last message content.
                        var content = firstMsg.Content;
                        if (content.Length > 50) content = content.Substring(0, 50) + "...";
                        c.Title = content;
                        
                        // Persist it so next time we don't query
                        await _convoRepo.UpdateAsync(c);
                    }
                    else
                    {
                         // No messages found -> Empty chat -> Skip
                         continue;
                    }
                }

                results.Add(new ChatConversationDto
                {
                    Id = c.Id.ToString(),
                    Title = c.Title!,
                    LastActivityAt = c.LastActivityAt,
                    IsPinned = c.IsPinned
                });
            }
            return results;
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(string conversationId)
        {
            var msgs = await _msgRepo.GetRecentByConversationAsync(conversationId, 100); // Fetch more for history
            return msgs.OrderBy(m => m.CreatedAt).Select(m => new ChatMessageDto
            {
                Id = m.Id.ToString(),
                Role = m.Role,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        public async Task DeleteConversationAsync(string studentId, string conversationId)
        {
            var convo = await _convoRepo.GetByIdAsync(conversationId);
            if (convo == null) return;
            
            // Validate ownership
            if (convo.StudentId != studentId) 
                throw new UnauthorizedAccessException("Cannot delete another student's conversation");

            await _convoRepo.DeleteAsync(conversationId);
            await _msgRepo.DeleteByConversationIdAsync(conversationId);
        }

        public async Task TogglePinAsync(string studentId, string conversationId, bool isPinned)
        {
            var convo = await _convoRepo.GetByIdAsync(conversationId);
            if (convo == null) return;
            
            if (convo.StudentId != studentId)
                throw new UnauthorizedAccessException();

            convo.IsPinned = isPinned;
            await _convoRepo.UpdateAsync(convo);
        }

        public async Task RenameConversationAsync(string studentId, string conversationId, string newTitle)
        {
            var convo = await _convoRepo.GetByIdAsync(conversationId);
            if (convo == null) return;
            
            if (convo.StudentId != studentId)
                throw new UnauthorizedAccessException();

            if (string.IsNullOrWhiteSpace(newTitle)) return;

            convo.Title = newTitle;
            await _convoRepo.UpdateAsync(convo);
        }
    }
}
