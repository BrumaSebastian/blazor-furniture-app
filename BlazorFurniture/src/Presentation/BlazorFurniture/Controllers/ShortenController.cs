using Microsoft.AspNetCore.Mvc;

namespace BlazorFurniture.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortenController : ControllerBase
{
    // In-memory store for demonstration. Replace with persistent storage in production.
    private static readonly Dictionary<string, string> _shortToOriginal = new();
    private static readonly string _baseUrl = "http://localhost:5080/api/Shorten/";

    [HttpPost]
    [Route("shorten")]
    public IActionResult Shorten([FromBody] ShortenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url) || !Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
            return BadRequest(new { message = "Invalid URL." });

        // Simple hash-based short code (for demo only)
        var shortCode = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "")
            .Replace("+", "")
            .Replace("/", "")
            .Substring(0, 8);

        _shortToOriginal[shortCode] = request.Url;

        var shortUrl = $"{_baseUrl}{shortCode}";
        return Ok(new ShortenResponse { ShortUrl = shortUrl });
    }

    [HttpGet("{code}")]
    public IActionResult RedirectToOriginal(string code)
    {
        if (_shortToOriginal.TryGetValue(code, out var originalUrl))
        {
            return Redirect(originalUrl);
        }
        return NotFound();
    }
}

public class ShortenRequest
{
    public string Url { get; set; }
}

public class ShortenResponse
{
    public string ShortUrl { get; set; }
}