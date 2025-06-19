using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Linq;

namespace FUNewsManagementSystem.Controllers
{
    [Authorize(Roles = "1")] // Chỉ Staff (Role = 1) được quản lý danh mục
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private const int PageSize = 10;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        public IActionResult Index(int pageNumber = 1, string searchKeyword = "")
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
                TotalItems = categories.Count
            };
            ViewBag.SearchKeyword = searchKeyword;

            return View(pagedCategories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewBag.ParentCategories = _categoryService.GetCategories();
            return PartialView("_Create", new Category());
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentCategories = _categoryService.GetCategories();
                return PartialView("_Create", category);
            }

            try
            {
                _categoryService.SaveCategory(category);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(short? id)
        {
            if (id == null)
                return NotFound();

            var category = _categoryService.GetCategoryById((short)id);
            if (category == null)
                return NotFound();

            ViewBag.ParentCategories = _categoryService.GetCategories();
            return PartialView("_Edit", category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, Category category)
        {
            if (id != category.CategoryId || !ModelState.IsValid)
            {
                ViewBag.ParentCategories = _categoryService.GetCategories();
                return PartialView("_Edit", category);
            }

            try
            {
                _categoryService.UpdateCategory(category);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(short id)
        {
            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null)
                    return Json(new { success = false, message = "Category not found." });

                _categoryService.DeleteCategory(category);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Categories/Details/5
        public IActionResult Details(short? id)
        {
            if (id == null)
                return NotFound();

            var category = _categoryService.GetCategoryById((short)id);
            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}