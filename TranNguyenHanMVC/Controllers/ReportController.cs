using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Linq;

namespace FUNewsManagementSystem.Controllers
{
    [Authorize(Roles = "0")] // Chỉ Admin (Role = 0) được xem báo cáo
    public class ReportController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private const int PageSize = 10;

        public ReportController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate, int pageNumber = 1)
        {
            var articles = _newsArticleService.GetArticles()
                .Where(a => a.NewsStatus == true)
                .OrderByDescending(a => a.CreatedDate)
                .ToList();

            if (startDate.HasValue && endDate.HasValue)
            {
                articles = articles
                    .Where(a => a.CreatedDate >= startDate.Value && a.CreatedDate <= endDate.Value)
                    .ToList();
            }

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
            ViewBag.StartDate = startDate ?? DateTime.Now.AddDays(-30);
            ViewBag.EndDate = endDate ?? DateTime.Now;

            return View(pagedArticles);
        }
    }
}