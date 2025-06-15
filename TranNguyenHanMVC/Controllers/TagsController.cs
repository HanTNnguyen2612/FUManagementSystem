using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace FUNewsManagement.Controllers
{
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            var tags = _tagService.GetTags();
            return View(tags);
        }

        public IActionResult Details(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var tag = _tagService.GetTagById(id.Value);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tag tag)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _tagService.SaveTag(tag);
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        public IActionResult Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var tag = _tagService.GetTagById(id.Value);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Tag tag)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id != tag.TagId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _tagService.UpdateTag(tag);
                }
                catch
                {
                    if (_tagService.GetTagById(id) == null)
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        public IActionResult Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();

            var tag = _tagService.GetTagById(id.Value);
            if (tag == null)
                return NotFound();

            return View(tag);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login", "Account");

            var tag = _tagService.GetTagById(id);
            if (tag != null)
            {
                _tagService.DeleteTag(tag);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}