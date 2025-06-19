using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Linq;

namespace FUNewsManagementSystem.Controllers
{
    [Authorize(Roles = "1")] // Chỉ Staff (Role = 1) được quản lý tag
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;
        private const int PageSize = 10;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public IActionResult Index(int pageNumber = 1, string searchKeyword = "")
        {
            var tags = string.IsNullOrEmpty(searchKeyword)
                ? _tagService.GetTags()
                : _tagService.SearchTags(searchKeyword);

            var pagedTags = tags
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.PagingInfo = new
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = tags.Count
            };
            ViewBag.SearchKeyword = searchKeyword;

            return View(pagedTags);
        }

        public IActionResult Create()
        {
            return PartialView("_Create", new Tag());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", tag);

            try
            {
                _tagService.SaveTag(tag);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tag = _tagService.GetTagById((int)id);
            if (tag == null)
                return NotFound();

            return PartialView("_Edit", tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Tag tag)
        {
            if (id != tag.TagId || !ModelState.IsValid)
                return PartialView("_Edit", tag);

            try
            {
                _tagService.UpdateTag(tag);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var tag = _tagService.GetTagById(id);
                if (tag == null)
                    return Json(new { success = false, message = "Tag not found." });

                _tagService.DeleteTag(tag);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tag = _tagService.GetTagById((int)id);
            if (tag == null)
                return NotFound();

            return View(tag);
        }
    }
}