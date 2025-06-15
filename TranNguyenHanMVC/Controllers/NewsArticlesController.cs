using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Collections.Generic;
using System.Linq;

namespace FUNewsManagement.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public NewsArticlesController(INewsArticleService newsArticleService, ICategoryService categoryService, ITagService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "2") // Lecturer
            {
                var articles = _newsArticleService.GetNewsArticles().Where(a => a.NewsStatus == true).ToList();
                return View(articles);
            }
            else if (userRole == "1") // Staff
            {
                var userId = short.Parse(HttpContext.Session.GetString("UserId"));
                var articles = _newsArticleService.GetNewsArticlesByStaff(userId);
                return View(articles);
            }
            else if (userRole == "0") // Admin
            {
                var articles = _newsArticleService.GetNewsArticles();
                return View(articles);
            }
            else
            {
                var articles = _newsArticleService.GetNewsArticles().Where(a => a.NewsStatus == true).ToList();
                return View(articles);
            }
        }

        public IActionResult Details(string id)
        {
            var article = _newsArticleService.GetNewsArticleById(id);
            if (article == null || (HttpContext.Session.GetString("UserRole") == "2" && article.NewsStatus != true))
                return NotFound();

            return View(article);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            ViewData["CategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName");
            ViewData["Tags"] = new Microsoft.AspNetCore.Mvc.Rendering.MultiSelectList(_tagService.GetTags(), "TagId", "TagName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NewsArticle article, List<int> tagIds)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                article.CreatedById = short.Parse(HttpContext.Session.GetString("UserId"));
                _newsArticleService.SaveNewsArticle(article, tagIds);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", article.CategoryId);
            ViewData["Tags"] = new Microsoft.AspNetCore.Mvc.Rendering.MultiSelectList(_tagService.GetTags(), "TagId", "TagName", tagIds);
            return View(article);
        }

        public IActionResult Edit(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            var article = _newsArticleService.GetNewsArticleById(id);
            if (article == null)
                return NotFound();

            ViewData["CategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", article.CategoryId);
            ViewData["Tags"] = new Microsoft.AspNetCore.Mvc.Rendering.MultiSelectList(_tagService.GetTags(), "TagId", "TagName", article.Tags.Select(t => t.TagId).ToList());
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, NewsArticle article, List<int> tagIds)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id != article.NewsArticleId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    article.UpdatedById = short.Parse(HttpContext.Session.GetString("UserId"));
                    _newsArticleService.UpdateNewsArticle(article, tagIds);
                }
                catch
                {
                    if (_newsArticleService.GetNewsArticleById(id) == null)
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", article.CategoryId);
            ViewData["Tags"] = new Microsoft.AspNetCore.Mvc.Rendering.MultiSelectList(_tagService.GetTags(), "TagId", "TagName", tagIds);
            return View(article);
        }

        public IActionResult Delete(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            var article = _newsArticleService.GetNewsArticleById(id);
            if (article == null)
                return NotFound();

            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            var article = _newsArticleService.GetNewsArticleById(id);
            if (article != null)
            {
                _newsArticleService.DeleteNewsArticle(article);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}