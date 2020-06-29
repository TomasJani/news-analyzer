using System.Collections.Generic;
using Api.Models;
using Api.Services;
using Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ArticleService _articleService;

        public ArticlesController(ArticleService articleService)
        {
            _articleService = articleService;
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
        public ActionResult<Article> Create(Article article)
        {
            _articleService.Create(article);

            return CreatedAtRoute("GetArticle", new {id = article.Id}, article);
        }
        
        [HttpPost("raw")]
        public ActionResult<Article> Create(RawArticle article)
        {
            var newArticle = _articleService.Create(article);

            return CreatedAtRoute("GetArticle", new {id = newArticle.Id}, newArticle);
        }

        [HttpPut("{id:length(24)}")]
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