using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ReceiptController : Controller
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptDocumentDto>>> GetAll([FromQuery] ReceiptFilterDto filter)
        {
            try
            {
                var receiptDocuments = await _receiptService.GetAllReceiptDocumentsAsync(filter);
                return Ok(receiptDocuments);
            }   
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ReceiptDocumentDto>> GetById(Guid id)
        {
            try
            {
                var receiptDocument = await _receiptService.GetReceiptDocumentByIdAsync(id);
                if (receiptDocument == null) return NotFound();
                return receiptDocument;
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReceiptDocumentCreateDto receiptDto)
        {
            try
            {
                var receiptId = await _receiptService.CreateReceiptDocumentAsync(receiptDto);
                // return CreatedAtAction(nameof(GetById), new { id = receiptId }, null);
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
        public async Task<IActionResult> Update(Guid id, [FromBody] ReceiptDocumentUpdateDto receiptDto)
        {
            try
            {
                if (id != receiptDto.Id) return BadRequest();
                await _receiptService.UpdateReceiptDocumentAsync(receiptDto);
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
                await _receiptService.DeleteReceiptDocumentAsync(id);
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
