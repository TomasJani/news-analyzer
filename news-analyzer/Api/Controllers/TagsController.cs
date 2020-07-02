using System.Collections.Generic;
using System.Net.Mime;
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
        public ActionResult<List<Tag>> Get() =>
            _tagService.Get();
        
        [HttpGet("{id:length(24)}", Name = "GetTag")]
        public ActionResult<Tag> Get(string id)
        {
            var tag = _tagService.Get(id);
        
            if (tag == null)
            {
                return NotFound();
            }
        
            return tag;
        }
        
        [HttpPost("search")]
        [Consumes("application/json")]
        public ActionResult<List<Tag>> Search([FromBody] string name)
        {
            var tags = _tagService.Search(name);

            if (tags.Count == 0)
            {
                return NotFound();
            }

            return tags;
        }

        [HttpPost]
        [Consumes("application/json")]
        public ActionResult<Tag> Create(Tag tag)
        {
            if (_tagService.NameExists(tag.Name))
                return Conflict();
            
            _tagService.Create(tag);

            return CreatedAtRoute("GetTag", new { id = tag.Id }, tag);
        }

        [HttpPut("{id:length(24)}")]
        [Consumes("application/json")]
        public IActionResult Update(string id, Tag tagIn)
        {
            var tag = _tagService.Get(id);

            if (tag == null)
            {
                return NotFound();
            }

            _tagService.Update(id, tagIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var tag = _tagService.Get(id);

            if (tag == null)
            {
                return NotFound();
            }

            _tagService.Remove(tag.Id);

            return NoContent();
        }
    }
}