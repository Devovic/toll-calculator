using Microsoft.AspNetCore.Mvc;
using TollCalculation.API.Dtos;
using TollCalculation.Core.Interfaces;
using TollCalculation.Core.Services;

namespace TollCalculation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TollController : ControllerBase
    {
        private readonly ITollFeeCalculatorService _tollFeeCalculatorService;
        private readonly ITimeConversionService _timeConversionService;

        public TollController(ITollFeeCalculatorService tollFeeCalculatorService, ITimeConversionService timeConversionService)
        {
            _tollFeeCalculatorService = tollFeeCalculatorService;
            _timeConversionService = timeConversionService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateTollFee([FromBody] CalculateTollRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.VehicleType) ||
                request.Dates == null ||
                request.Dates.Length == 0)
            {
                return BadRequest("Invalid request. Please provide a valid vehicle type and date-time values.");
            }

            var toUtcDates = Array.ConvertAll(request.Dates, _timeConversionService.ConvertToUtc);

            var tollFeeResults = await _tollFeeCalculatorService.CalculateTool(request.VehicleType, toUtcDates);

            return Ok(tollFeeResults);
        }
    }
}
