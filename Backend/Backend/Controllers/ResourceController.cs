using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ResourceController : Controller
    {
        private readonly IResourceService _resourceService;

        public ResourceController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAll()
        {
            try
            {
                var resources = await _resourceService.GetAllResourcesAsync();
                return Ok(resources);
            }
            catch(Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResourceDto>> GetById(Guid id)
        {
            try
            {
                var resource = await _resourceService.GetResourceByIdAsync(id);
                if (resource == null) return NotFound();
                return resource;
            }
            catch(Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{state:int}")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAllByState(int state)
        {
            try
            {
                var resources = await _resourceService.GetAllResourcesByStateAsync(state);
                return Ok(resources);
            }
            catch(Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourceCreateDto resourceDto)
        {
            try
            {
                var resourceId = await _resourceService.CreateResourceAsync(resourceDto);
                return CreatedAtAction(nameof(GetById), new { id = resourceId }, null);
            }
            catch(Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ResourceUpdateDto resourceDto)
        {
            try
            {
                if (id != resourceDto.Id) return BadRequest();
                await _resourceService.UpdateResourceAsync(resourceDto);
                return NoContent();
            }
            catch(Exception ex)
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
                await _resourceService.DeleteResourceAsync(id);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }
    }
}
