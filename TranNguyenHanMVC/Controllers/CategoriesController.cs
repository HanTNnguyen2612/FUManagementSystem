using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace FUNewsManagementSystem.Controllers
{
    [Authorize(Roles = "0,1")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;
        private const int PageSize = 10;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(int pageNumber = 1, string searchKeyword = "")
        {
            try
            {
                var categories = string.IsNullOrEmpty(searchKeyword)
                    ? _categoryService.GetCategories()
                    : _categoryService.SearchCategories(searchKeyword);

                var pagedCategories = categories
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                ViewBag.PagingInfo = new
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = categories.Count()
                };
                ViewBag.SearchKeyword = searchKeyword;
                ViewBag.AllCategories = _categoryService.GetCategories().ToList();

                return View(pagedCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category index with search keyword: {Keyword}, page: {Page}", searchKeyword, pageNumber);
                return View("Error");
            }
        }

        public IActionResult Create()
        {
            try
            {
                ViewBag.AllCategories = _categoryService.GetCategories().ToList();
                return PartialView("_Create", new Category { IsActive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create view.");
                return Json(new { success = false, message = "Error loading create form." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating category: {Name}", category.CategoryName);
                return Json(new { success = false, message = "Invalid data." });
            }

            try
            {
                _categoryService.SaveCategory(category);
                _logger.LogInformation("Category {Name} created.", category.CategoryName);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {Name}.", category.CategoryName);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Edit(short id)
        {
            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null)
                    return NotFound();

                ViewBag.AllCategories = _categoryService.GetCategories().ToList();
                return PartialView("_Edit", category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Edit view for category ID: {CategoryId}", id);
                return Json(new { success = false, message = "Error loading edit form." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, Category category)
        {
            if (id != category.CategoryId || !ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state or ID mismatch for editing category ID: {CategoryId}", id);
                return RedirectToAction("Index", new { error = "Error loading data." });
            }

            try
            {
                _categoryService.UpdateCategory(category);
                _logger.LogInformation("Category {Name} updated.", category.CategoryName);
                return RedirectToAction("Index", new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category ID: {CategoryId}", id);
                return RedirectToAction("Index", new { error = "Error updating category." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(short id)
        {
            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    _logger.LogWarning("Category not found for deletion, ID: {CategoryId}", id);
                    return RedirectToAction("Index", new { error = "Category not found." });
                }

                _categoryService.DeleteCategory(category);
                _logger.LogInformation("Category {Name} deleted.", category.CategoryName);
                return RedirectToAction("Index", new { success = true, message = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category ID: {CategoryId}", id);
                return RedirectToAction("Index", new { error = "Error deleting category." });
            }
        }
    }
}