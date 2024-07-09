using Microsoft.AspNetCore.Mvc;
using NFLDepthCharts.API.DTOs;
using NFLDepthCharts.API.Services;

namespace NFLDepthCharts.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepthChartController : ControllerBase
    {
        private readonly IDepthChartService _depthChartService;
        private readonly ILogger<DepthChartController> _logger;

        public DepthChartController(IDepthChartService depthChartService, ILogger<DepthChartController> logger)
        {
            _depthChartService = depthChartService;
            _logger = logger;
        }

        [HttpPost("addPlayerToDepthChart")]
        public async Task<IActionResult> AddPlayerToDepthChart([FromBody] AddPlayerToDepthChartDto dto)
        {
            _logger.LogInformation(nameof(AddPlayerToDepthChart));

            var result = await _depthChartService.AddPlayerToDepthChart(dto);
            return result ? CreatedAtAction(nameof(AddPlayerToDepthChart), null) : BadRequest();
        }

        [HttpDelete("removePlayerFromDepthChart")]
        public async Task<ActionResult<PlayerDto>> RemovePlayerFromDepthChart(string position, int playerNumber)
        {
            _logger.LogInformation(nameof(RemovePlayerFromDepthChart));

            var player = await _depthChartService.RemovePlayerFromDepthChart(position, playerNumber);
            return player != null ? Ok(player) : NotFound();
        }

        [HttpGet("getBackups")]
        public async Task<ActionResult<List<PlayerDto>>> GetBackups(string position, int playerNumber)
        {
            _logger.LogInformation(nameof(GetBackups));

            var backups = await _depthChartService.GetBackups(position, playerNumber);
            return Ok(backups);
        }

        [HttpGet("getFullDepthChart")]
        public async Task<ActionResult<FullDepthChartDto>> GetFullDepthChart()
        {
            _logger.LogInformation(nameof(GetFullDepthChart));

            var depthChart = await _depthChartService.GetFullDepthChart();
            return Ok(depthChart);
        }
    }
}
