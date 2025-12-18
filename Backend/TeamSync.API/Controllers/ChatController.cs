using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamSync.Application.Interfaces.Services;
using TeamSync.Application.DTOs.Chat;

namespace TeamSync.API.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not found in token.");

        // GET: /api/projects/{projectId}/chat
        [HttpGet]
        public async Task<IActionResult> GetChats(string projectId,
            [FromQuery] int skip = 0,
            [FromQuery] int limit = 20)
        {
            var chats = await _chatService.GetProjectChatsAsync(UserId, projectId, skip, limit);

            return Ok(new
            {
                success = true,
                data = chats
            });
        }

        // POST: /api/projects/{projectId}/chat
        [HttpPost]
        public async Task<IActionResult> SendMessage(
            string projectId,
            [FromBody] CreateChatMessageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Message cannot be empty");

            var created = await _chatService.CreateMessageAsync(
                projectId,
                UserId,
                dto.Message
            );

            return Ok(new
            {
                success = true,
                data = created
            });
        }
    }
}
