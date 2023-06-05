using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Serilog.Context;


using E.Shop.Gateway.Api.Logs;

namespace E.Shop.Gateway.Api.Controllers
{
    [ApiController]
    [Route("logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogger<LogsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Log([FromBody] LogRequest request)
        {
            _logger.LogInformation("Log from app {@request}", request);
            using (LogContext.PushProperty("Application", request.Application))
            using (LogContext.PushProperty("UserId", request.UserId))
            {
                switch (request.Level)
                {
                    case LogEventLevel.Information:
                        _logger.LogInformation(request.Message);
                        break;
                    case LogEventLevel.Warning:
                        _logger.LogWarning(request.Message);
                        break;
                    case LogEventLevel.Error:
                        _logger.LogError(request.Message);
                        break;
                }
            }
        }
    }
}