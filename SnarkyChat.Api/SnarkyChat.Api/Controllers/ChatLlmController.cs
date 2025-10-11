using Microsoft.AspNetCore.Mvc;
using SnarkyChat.Api.Models;
using SnarkyChat.Api.Services;

namespace SnarkyChat.Api.Controllers
{
    [ApiController]
    [Route("api/llm")]
    public class ChatLlmController : ControllerBase
    {
        private readonly LlmClientService _llm;
        
        public ChatLlmController(LlmClientService llm) => _llm = llm;

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest req)
        {
            var reply = await _llm.ChatAsync(req);
            return Ok(new { reply });
        }

        [HttpPost("stream")]
        public async Task Stream([FromBody] ChatRequest req, CancellationToken ct)
        {
            Response.Headers.ContentType = "text/event-stream";
            await _llm.StreamAsync(req, Response, ct);
        }
    }
}
