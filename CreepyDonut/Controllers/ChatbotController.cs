using CreepyDonut.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly OpenAiService _openAi;
    private readonly ProductService _productService;

    public ChatController(OpenAiService openAi, ProductService productService)
    {
        _openAi = openAi;
        _productService = productService;
    }

    public class ChatRequest { public string Message { get; set; } }
    public class ChatResponse { public string Reply { get; set; } }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatRequest request)
    {
        var products = await _productService.GetAllAsync(); 
        string donutList = string.Join("\n", products.Select(p => $"- {p.Name}: {p.Description}"));

        var reply = await _openAi.AskChatGPT(request.Message, donutList);

        return Ok(new ChatResponse { Reply = reply });
    }
}
