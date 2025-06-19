using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Linq;

namespace FUNewsManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private const int PageSize = 10;

        public HomeController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            var articles = _newsArticleService.GetArticles()
                .Where(a => a.NewsStatus.HasValue && a.NewsStatus.Value && // Kiểm tra NewsStatus == true
                           a.Category != null && a.Category.IsActive.HasValue && a.Category.IsActive.Value) // Kiểm tra Category.IsActive == true
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
                TotalItems = articles.Count
            };

            return View(pagedArticles);
        }
    }
}