using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FUNewsManagementSystem.Controllers
{
    [Authorize(Roles = "0,1")]
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ITagService _tagService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<NewsArticlesController> _logger;
        private const int PageSize = 10;

        public NewsArticlesController(
            INewsArticleService newsArticleService,
            ITagService tagService,
            ICategoryService categoryService,
            ILogger<NewsArticlesController> logger)
        {
            _newsArticleService = newsArticleService ?? throw new ArgumentNullException(nameof(newsArticleService));
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(int pageNumber = 1, string searchKeyword = "")
        {
            try
            {
                var articles = string.IsNullOrEmpty(searchKeyword)
                    ? _newsArticleService.GetArticles()
                    : _newsArticleService.SearchArticles(searchKeyword)
                    .Where(a => a.NewsStatus.HasValue && a.NewsStatus.Value &&
                            a.Category != null && a.Category.IsActive.HasValue && a.Category.IsActive.Value)
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();

                var pagedArticles = articles
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                ViewBag.PagingInfo = new
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = articles.Count()
                };
                ViewBag.SearchKeyword = searchKeyword;

                return View(pagedArticles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading article index with search keyword: {Keyword}, page: {Page}", searchKeyword, pageNumber);
                return View("Error");
            }
        }

        public IActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var article = _newsArticleService.GetArticleById(id);
                if (article == null)
                    return NotFound();

                return View(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading details for article ID: {ArticleId}", id);
                return View("Error");
            }
        }

        public IActionResult Create()
        {
            try
            {
                ViewBag.Tags = _tagService.GetTags()?.ToList() ?? new List<Tag>();
                ViewBag.Categories = _categoryService.GetCategories()?.ToList() ?? new List<Category>();
                return PartialView("_Create", new NewsArticle());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create view.");
                return RedirectToAction("Index", new { error = "Error loading create form." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NewsArticle article, int[] selectedTagIds)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating article: {Title}", article.NewsTitle);
                ViewBag.Tags = _tagService.GetTags()?.ToList() ?? new List<Tag>();
                ViewBag.Categories = _categoryService.GetCategories()?.ToList() ?? new List<Category>();
                return PartialView("_Create", article);
            }

            try
            {
                article.CreatedDate = DateTime.Now;
                article.CreatedById = short.Parse(User.FindFirst("AccountId")?.Value ?? "0");
                article.ModifiedDate = DateTime.Now;
                article.UpdatedById = article.CreatedById;

                if (selectedTagIds != null && selectedTagIds.Any())
                {
                    article.Tags = _tagService.GetTags()
                        .Where(t => selectedTagIds.Contains(t.TagId))
                        .ToList();
                }
                else
                {
                    article.Tags = new List<Tag>();
                }

                _newsArticleService.SaveArticle(article);
                _logger.LogInformation("Article {Title} created by user ID {UserId}.", article.NewsTitle, article.CreatedById);
                return RedirectToAction("Index", new {success = true, message = " create successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article {Title}.", article.NewsTitle);
                return RedirectToAction("Index", new { success = false, message = "create new article failed" });
            }
        }

        public IActionResult Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var article = _newsArticleService.GetArticleById(id);
                if (article == null)
                    return NotFound();

                ViewBag.Tags = _tagService.GetTags()?.ToList() ?? new List<Tag>();
                ViewBag.Categories = _categoryService.GetCategories()?.ToList() ?? new List<Category>();
                return PartialView("_Edit", article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Edit view for article ID: {ArticleId}", id);
                return Json(new { success = false, message = "Error loading edit form." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, NewsArticle article, int[] selectedTagIds)
        {
            if (string.IsNullOrEmpty(id) || !id.Equals(article.NewsArticleId) || !ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state or ID mismatch for editing article ID: {ArticleId}", id);
                ViewBag.Tags = _tagService.GetTags()?.ToList() ?? new List<Tag>();
                ViewBag.Categories = _categoryService.GetCategories()?.ToList() ?? new List<Category>();
                return PartialView("_Edit", article);
            }

            try
            {
                var existingArticle = _newsArticleService.GetArticleById(id);
                if (existingArticle == null)
                {
                    _logger.LogWarning("Article not found for editing, ID: {ArticleId}", id);
                    return RedirectToAction("Index", new { error = "Article not found." });
                }

                existingArticle.NewsTitle = article.NewsTitle;
                existingArticle.Headline = article.Headline;
                existingArticle.NewsContent = article.NewsContent;
                existingArticle.NewsSource = article.NewsSource;
                existingArticle.CategoryId = article.CategoryId;
                existingArticle.NewsStatus = article.NewsStatus;
                existingArticle.ModifiedDate = DateTime.Now;
                existingArticle.UpdatedById = short.Parse(User.FindFirst("AccountId")?.Value ?? "0");

                existingArticle.Tags = selectedTagIds != null && selectedTagIds.Any()
                    ? _tagService.GetTags().Where(t => selectedTagIds.Contains(t.TagId)).ToList()
                    : new List<Tag>();

                _newsArticleService.UpdateArticle(existingArticle);
                _logger.LogInformation("Article {Title} updated by user ID {UserId}.", existingArticle.NewsTitle, User.FindFirst("AccountId")?.Value);
                return RedirectToAction("Index", new { success = "Article updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article ID: {ArticleId}", id);
                return RedirectToAction("Index", new { error = "Error updating article." });
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
                {
                    _logger.LogWarning("Article not found for deletion, ID: {ArticleId}", id);
                    return RedirectToAction("Index", new { error = "Article not found." });
                }

                _newsArticleService.DeleteArticle(article);
                _logger.LogInformation("Article {Title} deleted by user ID {UserId}.", article.NewsTitle, User.FindFirst("AccountId")?.Value);
                return RedirectToAction("Index", new { success = "Article deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article ID: {ArticleId}", id);
                return RedirectToAction("Index", new { error = "Error deleting article." });
            }
        }

        [Authorize(Roles = "0")]
        public IActionResult ArticleReport(DateTime? startDate, DateTime? endDate, int pageNumber = 1)
        {
            try
            {
                ViewBag.PagingInfo = new
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = 0
                };
                ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

                if (startDate.HasValue && endDate.HasValue)
                {
                    var articles = _newsArticleService.GenerateReport(startDate.Value, endDate.Value);

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

                    return View(pagedArticles);
                }

                return View(new List<NewsArticle>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating article report from {StartDate} to {EndDate}", startDate, endDate);
                return View("Error");
            }
        }
    }
}