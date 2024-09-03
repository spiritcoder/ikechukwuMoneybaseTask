using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Moneybase.core.Interfaces.Services;
using Moneybase.core.Models;
using Newtonsoft.Json;

namespace ChatApi.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{

    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateChatSession([FromBody] ChatSessionRequest chatSessionRequest)
    {
        var session = await _chatService.CreateChatSessionAsync(chatSessionRequest);
        if (session == null)
        {
            return StatusCode(503, "Queue is full or overflow is unavailable.");
        }
        return Ok(session);
    }

    [HttpGet("poll/{sessionId}")]
    public async Task<IActionResult> PollSession(string sessionId)
    {
        var status = await _chatService.PollSession(sessionId);
        var statusObject = new
        {
            status,
        };

        return Ok(statusObject);
    }
}


