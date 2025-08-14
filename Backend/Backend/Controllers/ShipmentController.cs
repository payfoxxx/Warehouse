using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ShipmentController : Controller
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentDocumentDto>>> GetAll([FromQuery] ShipmentFilterDto filter)
        {
            try
            {
                var shipmentDocuments = await _shipmentService.GetAllShipmentDocumentsAsync(filter);
                return Ok(shipmentDocuments);
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ShipmentDocumentDto>> GetById(Guid id)
        {
            try
            {
                var shipmentDocument = await _shipmentService.GetShipmentDocumentByIdAsync(id);
                if (shipmentDocument == null) return NotFound();
                return shipmentDocument;
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShipmentDocumentCreateDto shipmentDto)
        {
            try
            {
                var shipmentId = await _shipmentService.CreateShipmentDocumentAsync(shipmentDto);
                // return CreatedAtAction(nameof(GetById), new { id = shipmentId }, null);
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
        public async Task<IActionResult> Update(Guid id, [FromBody] ShipmentDocumentUpdateDto shipmentDto)
        {
            try
            {
                if (id != shipmentDto.Id) return BadRequest();
                await _shipmentService.UpdateShipmentDocumentAsync(shipmentDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id}/sign")]
        public async Task<IActionResult> Sign(Guid id, [FromBody] ShipmentDocumentUpdateDto dto)
        {
            try
            {
                await _shipmentService.SignAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id}/revoke")]
        public async Task<IActionResult> Revoke(Guid id)
        {
            try
            {
                await _shipmentService.RevokeAsync(id);
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
                await _shipmentService.DeleteShipmentDocumentAsync(id);
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
