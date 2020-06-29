using System.Collections.Generic;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly AuthorService _authorService;

        public AuthorsController(AuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public ActionResult<List<Author>> Get() =>
            _authorService.Get();
        
        [HttpGet("{id:length(24)}", Name = "GetAuthor")]
        public ActionResult<Author> Get(string id)
        {
            var author = _authorService.Get(id);
        
            if (author == null)
            {
                return NotFound();
            }
        
            return author;
        }
        
        [HttpPost("search")]
        public ActionResult<List<Author>> Search([FromBody] string name)
        {
            var authors = _authorService.Search(name);

            if (authors.Count == 0)
            {
                return NotFound();
            }

            return authors;
        }

        [HttpPost]
        public ActionResult<Author> Create(Author author)
        {
            _authorService.Create(author);

            return CreatedAtRoute("GetAuthor", new { id = author.Id }, author);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Author authorIn)
        {
            var author = _authorService.Get(id);

            if (author == null)
            {
                return NotFound();
            }

            _authorService.Update(id, authorIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var author = _authorService.Get(id);

            if (author == null)
            {
                return NotFound();
            }

            _authorService.Remove(author.Id);

            return NoContent();
        }
    }
}