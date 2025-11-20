using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RemoteReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheck;
        public HealthController(HealthCheckService healthCheck)
        {
            _healthCheck = healthCheck;
        }


        [HttpGet("live")]
        public async Task<IActionResult> Live(CancellationToken ct)
        {
            var report = await _healthCheck.CheckHealthAsync(x => x.Tags.Contains("live"), ct);

            var result = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    descricao = e.Value.Description,
                    erro = e.Value.Exception?.Message
                })
            };

            return report.Status == HealthStatus.Healthy ? Ok(result) : StatusCode(503, result);
        }


        [HttpGet("ready")]
        public async Task<IActionResult> Ready(CancellationToken ct)
        {
            var report = await _healthCheck.CheckHealthAsync(x => x.Tags.Contains("ready"), ct);

            var result = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    descricao = e.Value.Description,
                    erro = e.Value.Exception?.Message
                })
            };

            return report.Status == HealthStatus.Healthy ? Ok(result) : StatusCode(503, result);
        }
    }
}
