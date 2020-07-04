using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class TagsController : ControllerBase
    {
        private readonly TagService _tagService;
        private readonly ILogger<TagsController> _logger;

        public TagsController(TagService tagService, ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tag>>> Get() =>
            await _tagService.Get();
        
        [HttpGet("{id:length(24)}", Name = "GetTag")]
        public async Task<ActionResult<Tag>> Get(string id)
        {
            var tag = await _tagService.Get(id);
        
            if (tag == null)
            {
                return NotFound();
            }
        
            return tag;
        }
        
        [HttpPost("search")]
        [Consumes("application/json")]
        public async Task<ActionResult<List<Tag>>> Search([FromBody] string name)
        {
            var tags = await _tagService.Search(name);

            if (tags.Count == 0)
            {
                return NotFound();
            }

            return tags;
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<Tag>> Create(Tag tag)
        {
            if (await _tagService.NameExists(tag.Name))
                return Conflict();
            
            await _tagService.Create(tag);

            return CreatedAtRoute("GetTag", new { id = tag.Id }, tag);
        }

        [HttpPut("{id:length(24)}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(string id, Tag tagIn)
        {
            var tag = await _tagService.Get(id);

            if (tag == null)
            {
                return NotFound();
            }

            await _tagService.Update(id, tagIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var tag = await _tagService.Get(id);

            if (tag == null)
            {
                return NotFound();
            }

            await _tagService.Remove(tag.Id);

            return NoContent();
        }
    }
}