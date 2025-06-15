using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Services;
using System.Linq;

namespace FUNewsManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly INewsArticleService _newsArticleService;
        private readonly IConfiguration _configuration;

        public AccountController(ISystemAccountService systemAccountService, INewsArticleService newsArticleService, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _newsArticleService = newsArticleService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string AccountEmail, string AccountPassword)
        {
            if (ModelState.IsValid)
            {
                var adminEmail = _configuration["AdminCredentials:Email"];
                var adminPassword = _configuration["AdminCredentials:Password"];
                var adminRole = int.Parse(_configuration["AdminCredentials:Role"]);

                if (AccountEmail == adminEmail && AccountPassword == adminPassword)
                {
                    HttpContext.Session.SetString("UserId", "0");
                    HttpContext.Session.SetString("UserRole", adminRole.ToString());
                    HttpContext.Session.SetString("Username", "Admin");
                    return RedirectToAction("Index", "SystemAccounts");
                }

                var user = _systemAccountService.GetAccountByEmail(AccountEmail);
                if (user != null && user.AccountPassword == AccountPassword)
                {
                    HttpContext.Session.SetString("UserId", user.AccountId.ToString());
                    HttpContext.Session.SetString("UserRole", user.AccountRole.ToString());
                    HttpContext.Session.SetString("Username", user.AccountName);
                    if (user.AccountRole == 1)
                        return RedirectToAction("Index", "NewsArticles");
                    else if (user.AccountRole == 2)
                        return RedirectToAction("Index", "NewsArticles");
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        //public IActionResult Index()
        //{
        //    if (HttpContext.Session.GetString("UserRole") != "0")
        //        return RedirectToAction("Login");

        //    var accounts = _systemAccountService.GetAccounts();
        //    return View(accounts);
        //}

        public IActionResult Details(short? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "0")
                return RedirectToAction("Login");

            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetAccountById(id.Value);
            if (account == null)
                return NotFound();

            return View(account);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "0")
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SystemAccount account)
        {
            if (HttpContext.Session.GetString("UserRole") != "0")
                return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                _systemAccountService.SaveAccount(account);
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        public IActionResult Edit(short? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "0" && HttpContext.Session.GetString("UserId") != id.ToString())
                return RedirectToAction("Login");

            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetAccountById(id.Value);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, SystemAccount account)
        {
            if (HttpContext.Session.GetString("UserRole") != "0" && HttpContext.Session.GetString("UserId") != id.ToString())
                return RedirectToAction("Login");

            if (id != account.AccountId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _systemAccountService.UpdateAccount(account);
                }
                catch
                {
                    if (_systemAccountService.GetAccountById(id) == null)
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        public IActionResult Delete(short? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "0")
                return RedirectToAction("Login");

            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetAccountById(id.Value);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(short id)
        {
            if (HttpContext.Session.GetString("UserRole") != "0")
                return RedirectToAction("Login");

            var account = _systemAccountService.GetAccountById(id);
            if (account != null)
            {
                _systemAccountService.DeleteAccount(account);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            if (HttpContext.Session.GetString("UserRole") != "0")
                return RedirectToAction("Login");

            var articles = _newsArticleService.GetNewsArticles()
                .Where(a => a.CreatedDate >= (startDate ?? DateTime.MinValue) && a.CreatedDate <= (endDate ?? DateTime.MaxValue))
                .OrderByDescending(a => a.CreatedDate)
                .ToList();
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View(articles);
        }

        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("UserRole") != "1")
                return RedirectToAction("Login");

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !short.TryParse(userIdString, out short userId))
                return RedirectToAction("Login");

            var account = _systemAccountService.GetAccountById(userId);
            if (account == null)
                return NotFound();

            return View(account);
        }
    }
}