using BusinessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FUNewsManagementSystem.Controllers
{
    public class SystemAccountsController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly ILogger<SystemAccountsController> _logger;
        private readonly IConfiguration _configuration;
        private const int PageSize = 10;

        public SystemAccountsController(ISystemAccountService systemAccountService, ILogger<SystemAccountsController> logger, IConfiguration configuration)
        {
            _systemAccountService = systemAccountService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new SystemAccount());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(SystemAccount model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", model.AccountEmail);
                return View(model);
            }

            // Kiểm tra tài khoản trong database
            var account = _systemAccountService.GetAccountByEmail(model.AccountEmail);
            if (account != null)
            {
                // Kiểm tra mật khẩu (giả sử lưu dạng plain text, cần mã hóa thực tế)
                if (account.AccountPassword == model.AccountPassword)
                {
                    // Kiểm tra vai trò hợp lệ
                    if (!account.AccountRole.HasValue || account.AccountRole < 0 || account.AccountRole > 2)
                    {
                        ModelState.AddModelError("", "Invalid account role.");
                        _logger.LogWarning("Login failed: Invalid role for email {Email}.", model.AccountEmail);
                        return View(model);
                    }

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, account.AccountName ?? account.AccountEmail),
                        new Claim(ClaimTypes.Email, account.AccountEmail),
                        new Claim(ClaimTypes.Role, account.AccountRole.Value.ToString()),
                        new Claim("AccountId", account.AccountId.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    _logger.LogInformation("User {Email} (Role: {Role}) logged in successfully.", account.AccountEmail, account.AccountRole);
                    return RedirectToAction("RedirectBasedOnRole");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid password.");
                    _logger.LogWarning("Login failed: Invalid password for email {Email}.", model.AccountEmail);
                    return View(model);
                }
            }

            // Dự phòng: Kiểm tra tài khoản Admin từ appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (string.IsNullOrEmpty(adminEmail) || model.AccountEmail != adminEmail)
            {
                ModelState.AddModelError("", "Email not found.");
                _logger.LogWarning("Login failed: Email {Email} not found.", model.AccountEmail);
                return View(model);
            }

            if (string.IsNullOrEmpty(adminPassword) || model.AccountPassword != adminPassword)
            {
                ModelState.AddModelError("", "Invalid password.");
                _logger.LogWarning("Login failed: Invalid password for email {Email}.", model.AccountEmail);
                return View(model);
            }

            // Đăng nhập Admin từ appsettings.json
            var claimsAdmin = new[]
            {
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Email, adminEmail),
                new Claim(ClaimTypes.Role, "0"),
                new Claim("AccountId", "0")
            };

            var identityAdmin = new ClaimsIdentity(claimsAdmin, CookieAuthenticationDefaults.AuthenticationScheme);
            var principalAdmin = new ClaimsPrincipal(identityAdmin);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principalAdmin);

            _logger.LogInformation("Admin {Email} logged in successfully.", adminEmail);
            return RedirectToAction("RedirectBasedOnRole");
        }

        [Authorize]
        public IActionResult RedirectBasedOnRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            switch (role)
            {
                case "0": // Admin
                    return RedirectToAction("Index", "SystemAccounts");
                case "1": // Staff
                    return RedirectToAction("Index", "NewsArticles");
                case "2": // Lecturer
                    return RedirectToAction("Index", "Home");
                default:
                    return RedirectToAction("AccessDenied");
            }
        }

        [Authorize(Roles = "0")]
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

        [Authorize(Roles = "1")]
        public IActionResult Profile()
        {
            try
            {
                var accountId = short.Parse(User.FindFirst("AccountId")?.Value ?? "0");
                var account = _systemAccountService.GetAccountById(accountId);
                if (account == null)
                {
                    _logger.LogWarning("Account not found for ID: {AccountId}", accountId);
                    return NotFound();
                }

                return View(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile for user ID: {AccountId}", User.FindFirst("AccountId")?.Value);
                return View("Error");
            }
        }

        [Authorize(Roles = "1")]
        public IActionResult EditProfile()
        {
            try
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                _logger.LogInformation("EditProfile GET called, AccountId claim: {AccountId}, Role: {Role}", accountIdClaim, User.FindFirst(ClaimTypes.Role)?.Value);
                if (string.IsNullOrEmpty(accountIdClaim) || !short.TryParse(accountIdClaim, out short accountId))
                {
                    _logger.LogWarning("Invalid or missing AccountId claim");
                    return Unauthorized("Invalid user identity.");
                }

                var account = _systemAccountService.GetAccountById(accountId);
                if (account == null)
                {
                    _logger.LogWarning("Account not found for ID: {AccountId}", accountId);
                    return NotFound();
                }

                return PartialView("_EditProfile", account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading EditProfile view for user ID: {AccountId}", User.FindFirst("AccountId")?.Value);
                return Json(new { success = false, message = "Error loading edit form." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(SystemAccount account)
        {
            try
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;
                _logger.LogInformation("EditProfile POST called, AccountId claim: {AccountId}, Input AccountID: {InputAccountId}", accountIdClaim, account.AccountId);
                if (string.IsNullOrEmpty(accountIdClaim) || !short.TryParse(accountIdClaim, out short accountId) || account.AccountId != accountId)
                {
                    _logger.LogWarning("Invalid model state or ID mismatch for editing account ID: {AccountId}");
                    return Json(new { success = false, message = "Invalid user identity." });
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for editing account ID: {AccountId}", accountId);
                    return PartialView("_EditProfile", account);
                }

                var existingAccount = _systemAccountService.GetAccountById(accountId);
                if (existingAccount == null)
                {
                    _logger.LogWarning("Account not found for ID: {AccountId}", accountId);
                    return RedirectToAction("Index", "NewsArticle", new {message ="account not found"});
                }

                existingAccount.AccountName = account.AccountName;
                existingAccount.AccountEmail = account.AccountEmail;
                existingAccount.AccountPassword = account.AccountPassword;
                existingAccount.AccountRole = 1;

                _systemAccountService.UpdateAccount(existingAccount);
                _logger.LogInformation("Account {Email} updated by user ID: {AccountId}", existingAccount.AccountEmail, accountId);
                return RedirectToAction("Profile", new { message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account ID: {AccountId}", account.AccountId);
                return RedirectToAction("Profile", new { message = "Error updating profile." });
            }
        }

        [Authorize(Roles = "0")]
        public IActionResult Create()
        {
            return PartialView("_Create", new SystemAccount());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "0")]
        public IActionResult Create(SystemAccount account)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", account);

            try
            {
                _systemAccountService.SaveAccount(account);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "0")]
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
        [Authorize(Roles = "0")]
        public IActionResult Edit(short id, SystemAccount account)
        {
            if (id != account.AccountId || !ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data." });

            try
            {
                _systemAccountService.UpdateAccount(account);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account ID {AccountId}.", id);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "0")]
        public IActionResult Delete(short id)
        {
            try
            {
                var account = _systemAccountService.GetAccountById(id);
                if (account == null)
                    return Json(new { success = false, message = "Account not found." });

                _systemAccountService.DeleteAccount(account);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "0")]
        public IActionResult Details(short? id)
        {
            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetAccountById((short)id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out successfully.");
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}