using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace FUNewsManagement.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            var categories = _categoryService.GetCategories();
            return View(categories);
        }

        public IActionResult Details(short? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            ViewData["ParentCategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _categoryService.SaveCategory(category);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", category.ParentCategoryId);
            return View(category);
        }

        public IActionResult Edit(short? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
                return NotFound();

            ViewData["ParentCategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", category.ParentCategoryId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, Category category)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id != category.CategoryId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.UpdateCategory(category);
                }
                catch
                {
                    if (_categoryService.GetCategoryById(id) == null)
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName", category.ParentCategoryId);
            return View(category);
        }

        public IActionResult Delete(short? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(short id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            var category = _categoryService.GetCategoryById(id);
            if (category != null)
            {
                try
                {
                    _categoryService.DeleteCategory(category);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(category);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}