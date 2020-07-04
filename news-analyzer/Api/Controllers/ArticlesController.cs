using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
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
        public async Task<ActionResult<List<Article>>> Get() =>
            await _articleService.Get();

        [HttpGet("{id:length(24)}", Name = "GetArticle")]
        public async Task<ActionResult<Article>> Get(string id)
        {
            var article = await _articleService.Get(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        [HttpPost("search")]
        [Consumes("application/json")]
        public async Task<ActionResult<List<Article>>> Search([FromBody] string text)
        {
            var articles = await _articleService.Search(text);

            if (articles.Count == 0)
            {
                return NotFound();
            }

            return articles;
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<Article>> Create(Article article)
        {
            if (await _articleService.TitleExists(article.Title))
                return Conflict();
            
            await _articleService.Create(article);

            return CreatedAtRoute("GetArticle", new {id = article.Id}, article);
        }
        
        [HttpPost("raw")]
        [Consumes("application/json")]
        public async Task<ActionResult<Article>> Create(RawArticle article)
        {
            if (await _articleService.TitleExists(article.Title))
                return Conflict();
            
            var newArticle = await _articleService.Create(article);

            return CreatedAtRoute("GetArticle", new {id = newArticle.Id}, newArticle);
        }

        [HttpPut("{id:length(24)}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(string id, Article articleIn)
        {
            var article = await _articleService.Get(id);

            if (article == null)
            {
                return NotFound();
            }

            await _articleService.Update(id, articleIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var article = await _articleService.Get(id);

            if (article == null)
            {
                return NotFound();
            }

            await _articleService.Remove(article.Id);

            return NoContent();
        }
    }
}