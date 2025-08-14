using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MeasureUnitController : Controller
    {
        private readonly IMeasureUnitService _measureUnitService;

        public MeasureUnitController(IMeasureUnitService measureUnitService)
        {
            _measureUnitService = measureUnitService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeasureUnitDto>>> GetAll()
        {
            try
            {
                var measureUnits = await _measureUnitService.GetAllMeasureUnitsAsync();
                return Ok(measureUnits);
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MeasureUnitDto>> GetById(Guid id)
        {
            try
            {
                var measureUnit = await _measureUnitService.GetMeasureUnitByIdAsync(id);
                if (measureUnit == null) return NotFound();
                return measureUnit;
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{state:int}")]
        public async Task<ActionResult<IEnumerable<MeasureUnitDto>>> GetAllByState(int state)
        {
            try
            {
                var measureUnits = await _measureUnitService.GetAllMeasureUnitsByStateAsync(state);
                return Ok(measureUnits);
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MeasureUnitCreateDto measureUnitDto)
        {
            try
            {
                var measureUnitId = await _measureUnitService.CreateMeasureUnitAsync(measureUnitDto);
                return CreatedAtAction(nameof(GetById), new { id = measureUnitId }, null);
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MeasureUnitUpdateDto measureUnitDto)
        {
            try
            {
                if (id != measureUnitDto.Id) return BadRequest();
                await _measureUnitService.UpdateMeasureUnitAsync(measureUnitDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _measureUnitService.DeleteMeasureUnitAsync(id);
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
