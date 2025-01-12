using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhoBloberWebAPI.DB;

namespace PhoBloberWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly LoggerDBContext _dbContext;
        private readonly ILogger<LoggingController> _logger;

        public LoggingController(LoggerDBContext dbContext, ILogger<LoggingController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetLogs()
        {
            _logger.LogInformation("GetLogs called");

            var logs = _dbContext.LogItems.ToList();
            _logger.LogInformation($"Fetched {logs.Count} log items.");

            return Ok(logs);
        }

        [HttpGet("info")]
        public IActionResult LogInformation()
        {
            _logger.LogInformation("This is an informational log message.");
            return Ok("Information log recorded.");
        }

        [HttpGet("warning")]
        public IActionResult LogWarning()
        {
            _logger.LogWarning("This is a warning log message.");
            return Ok("Warning log recorded.");
        }

        [HttpGet("error")]
        public IActionResult LogError()
        {
            _logger.LogError("This is an error log message.");
            return Ok("Error log recorded.");
        }

        [HttpPost]
        public IActionResult CreateLog([FromBody] LogItem logItem)
        {
            _logger.LogInformation("CreateLog called");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid log item data received.");
                return BadRequest(ModelState);
            }

            logItem.Timestamp = DateTime.UtcNow;
            _dbContext.LogItems.Add(logItem);
            _dbContext.SaveChanges();

            _logger.LogInformation("Log item created successfully.");
            return CreatedAtAction(nameof(GetLogs), new { id = logItem.Id }, logItem);
        }
    }
}
