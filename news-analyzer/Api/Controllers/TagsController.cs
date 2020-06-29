using System.Collections.Generic;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagsController(TagService tagService)
        {
            _tagService = tagService;
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
        public ActionResult<Tag> Create(Tag tag)
        {
            _tagService.Create(tag);

            return CreatedAtRoute("GetTag", new { id = tag.Id }, tag);
        }

        [HttpPut("{id:length(24)}")]
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