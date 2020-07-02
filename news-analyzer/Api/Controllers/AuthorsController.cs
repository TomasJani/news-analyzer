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
    public class AuthorsController : ControllerBase
    {
        private readonly AuthorService _authorService;
        private readonly ILogger<AuthorsController> _logger;
        
        public AuthorsController(AuthorService authorService, ILogger<AuthorsController> logger)
        {
            _authorService = authorService;
            _logger = logger;
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
        [Consumes("application/json")]
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
        [Consumes("application/json")]
        public ActionResult<Author> Create(Author author)
        {
            if (_authorService.NameExists(author.Name))
                return Conflict();
            
            _authorService.Create(author);

            return CreatedAtRoute("GetAuthor", new {id = author.Id}, author);
        }

        [HttpPut("{id:length(24)}")]
        [Consumes("application/json")]
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