using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Linq;

namespace FUNewsManagementSystem.Controllers
{
    [Authorize(Roles = "1")] // Chỉ Staff (Role = 1) được quản lý bài báo
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private const int PageSize = 10;

        public NewsArticlesController(INewsArticleService newsArticleService,
                                     ICategoryService categoryService,
                                     ITagService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        public IActionResult Index(int pageNumber = 1, string searchKeyword = "")
        {
            var articles = string.IsNullOrEmpty(searchKeyword)
                ? _newsArticleService.GetArticles()
                : _newsArticleService.SearchArticles(searchKeyword);

            var pagedArticles = articles
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.PagingInfo = new
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = articles.Count
            };
            ViewBag.SearchKeyword = searchKeyword;

            return View(pagedArticles);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _categoryService.GetCategories();
            ViewBag.Tags = _tagService.GetTags();
            return PartialView("_Create", new NewsArticle());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NewsArticle article, int[] selectedTagIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetCategories();
                ViewBag.Tags = _tagService.GetTags();
                return PartialView("_Create", article);
            }

            try
            {
                var accountId = short.Parse(User.FindFirst("AccountId").Value);
                article.CreatedById = accountId;
                article.CreatedDate = DateTime.Now;
                _newsArticleService.SaveArticle(article, selectedTagIds?.ToList() ?? new List<int>()); // Chuyển int[] sang List<int>
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();

            var article = _newsArticleService.GetArticleById(id);
            if (article == null)
                return NotFound();

            ViewBag.Categories = _categoryService.GetCategories();
            ViewBag.Tags = _tagService.GetTags();
            ViewBag.SelectedTagIds = article.Tags.Select(t => t.TagId).ToArray();
            return PartialView("_Edit", article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, NewsArticle article, int[] selectedTagIds)
        {
            if (id != article.NewsArticleId || !ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetCategories();
                ViewBag.Tags = _tagService.GetTags();
                return PartialView("_Edit", article);
            }

            try
            {
                var accountId = short.Parse(User.FindFirst("AccountId").Value);
                article.UpdatedById = accountId;
                article.ModifiedDate = DateTime.Now;
                _newsArticleService.UpdateArticle(article, selectedTagIds?.ToList() ?? new List<int>()); // Chuyển int[] sang List<int>
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            try
            {
                var article = _newsArticleService.GetArticleById(id);
                if (article == null)
                    return Json(new { success = false, message = "Article not found." });

                _newsArticleService.DeleteArticle(article);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Details(string id)
        {
            if (id == null)
                return NotFound();

            var article = _newsArticleService.GetArticleById(id);
            if (article == null)
                return NotFound();

            return View(article);
        }
    }
}