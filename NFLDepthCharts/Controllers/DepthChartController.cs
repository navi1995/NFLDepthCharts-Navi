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

        public DepthChartController(IDepthChartService depthChartService)
        {
            _depthChartService = depthChartService;
        }

        [HttpPost("addPlayerToDepthChart")]
        public async Task<IActionResult> AddPlayerToDepthChart([FromBody] AddPlayerToDepthChartDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _depthChartService.AddPlayerToDepthChart(dto);
            return result ? CreatedAtAction(nameof(AddPlayerToDepthChart), null) : BadRequest();
        }

        [HttpDelete("removePlayerFromDepthChart")]
        public async Task<ActionResult<PlayerDto>> RemovePlayerFromDepthChart(string position, int playerNumber)
        {
            var player = await _depthChartService.RemovePlayerFromDepthChart(position, playerNumber);
            return player != null ? Ok(player) : NotFound();
        }

        [HttpGet("getBackups")]
        public async Task<ActionResult<List<PlayerDto>>> GetBackups(string position, int playerNumber)
        {
            var backups = await _depthChartService.GetBackups(position, playerNumber);
            return Ok(backups);
        }

        [HttpGet("getFullDepthChart")]
        public async Task<ActionResult<FullDepthChartDto>> GetFullDepthChart()
        {
            var depthChart = await _depthChartService.GetFullDepthChart();
            return Ok(depthChart);
        }
    }
}
