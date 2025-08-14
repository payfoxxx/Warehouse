using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BalanceController : Controller
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BalanceDto>>> GetAll([FromQuery] BalanceFilterDto filter)
        {
            try
            {
                var balanceDocuments = await _balanceService.GetAllBalancesAsync(filter);
                return Ok(balanceDocuments);
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BalanceDto>> GetById(Guid id)
        {
            try
            {
                var balanceDocument = await _balanceService.GetBalanceByIdAsync(id);
                if (balanceDocument == null) return NotFound();
                return balanceDocument;
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BalanceCreateDto balanceDto)
        {
            try
            {
                var balanceId = await _balanceService.CreateBalanceAsync(balanceDto);
                // return CreatedAtAction(nameof(GetById), new { id = balanceId }, null);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BalanceUpdateDto balanceDto)
        {
            try
            {
                if (id != balanceDto.Id) return BadRequest();
                await _balanceService.UpdateBalanceAsync(balanceDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }
    }
}
