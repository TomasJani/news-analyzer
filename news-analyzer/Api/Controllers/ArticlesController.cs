using System.Collections.Generic;
using System.Net.Mime;
using Api.Models;
using Api.Services;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleService _articleService;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(ArticleService articleService, ILogger<ArticlesController> logger)
        {
            _articleService = articleService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<Article>> Get() =>
            _articleService.Get();

        [HttpGet("{id:length(24)}", Name = "GetArticle")]
        public ActionResult<Article> Get(string id)
        {
            var article = _articleService.Get(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        [HttpPost("search")]
        [Consumes("application/json")]
        public ActionResult<List<Article>> Search([FromBody] string text)
        {
            var articles = _articleService.Search(text);

            if (articles.Count == 0)
            {
                return NotFound();
            }

            return articles;
        }

        [HttpPost]
        [Consumes("application/json")]
        public ActionResult<Article> Create(Article article)
        {
            if (_articleService.TitleExists(article.Title))
                return Conflict();
            
            _articleService.Create(article);

            return CreatedAtRoute("GetArticle", new {id = article.Id}, article);
        }
        
        [HttpPost("raw")]
        [Consumes("application/json")]
        public ActionResult<Article> Create(RawArticle article)
        {
            if (_articleService.TitleExists(article.Title))
                return Conflict();
            
            var newArticle = _articleService.Create(article);

            return CreatedAtRoute("GetArticle", new {id = newArticle.Id}, newArticle);
        }

        [HttpPut("{id:length(24)}")]
        [Consumes("application/json")]
        public IActionResult Update(string id, Article articleIn)
        {
            var article = _articleService.Get(id);

            if (article == null)
            {
                return NotFound();
            }

            _articleService.Update(id, articleIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var article = _articleService.Get(id);

            if (article == null)
            {
                return NotFound();
            }

            _articleService.Remove(article.Id);

            return NoContent();
        }
    }
}