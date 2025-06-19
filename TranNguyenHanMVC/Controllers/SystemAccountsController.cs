using BusinessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace FUNewsManagementSystem.Controllers
{
    [Authorize(Roles = "0")] // Chỉ Admin (Role = 0) được quản lý tài khoản
    public class AccountsController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly IConfiguration _configuration;
        private const int PageSize = 10;

        public AccountsController(ISystemAccountService systemAccountService, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new SystemAccount());
        }

        [HttpPost]
        public IActionResult Login(SystemAccount account)
        {
            if (!ModelState.IsValid)
                return View(account);

            var existingAccount = _systemAccountService.GetAccountByEmail(account.AccountEmail);
            if (existingAccount == null || existingAccount.AccountPassword != account.AccountPassword)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(account);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, existingAccount.AccountName),
                new Claim(ClaimTypes.Email, existingAccount.AccountEmail),
                new Claim(ClaimTypes.Role, existingAccount.AccountRole.ToString()),
                new Claim("AccountId", existingAccount.AccountId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Index(int pageNumber = 1, string searchKeyword = "")
        {
            var accounts = string.IsNullOrEmpty(searchKeyword)
                ? _systemAccountService.GetAccounts()
                : _systemAccountService.SearchAccounts(searchKeyword);

            var pagedAccounts = accounts
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.PagingInfo = new
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = accounts.Count
            };
            ViewBag.SearchKeyword = searchKeyword;

            return View(pagedAccounts);
        }

        [HttpGet]
        [Authorize(Roles = "1")]
        public IActionResult Profile()
        {
            var accountId = short.Parse(User.FindFirst("AccountId").Value);
            var account = _systemAccountService.GetAccountById(accountId);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        public IActionResult Profile(SystemAccount account)
        {
            if (!ModelState.IsValid)
                return View(account);

            try
            {
                _systemAccountService.UpdateAccount(account);
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(account);
            }
        }

        public IActionResult Create()
        {
            return PartialView("_Create", new SystemAccount());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SystemAccount account)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", account);

            try
            {
                _systemAccountService.SaveAccount(account);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Edit(short? id)
        {
            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetAccountById((short)id);
            if (account == null)
                return NotFound();

            return PartialView("_Edit", account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, SystemAccount account)
        {
            if (id != account.AccountId || !ModelState.IsValid)
                return PartialView("_Edit", account);

            try
            {
                _systemAccountService.UpdateAccount(account);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(short id)
        {
            try
            {
                var account = _systemAccountService.GetAccountById(id);
                if (account == null)
                    return Json(new { success = false, message = "Account not found." });

                _systemAccountService.DeleteAccount(account);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Details(short? id)
        {
            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetAccountById((short)id);
            if (account == null)
                return NotFound();

            return View(account);
        }
    }
}