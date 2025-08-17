using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Company.Demo.PL.ViewModels.Account;
using Company.Demo.PL.Services;

namespace Company.Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            // Redirect if user is already logged in
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError("", "يرجى إدخال البريد الإلكتروني وكلمة المرور");
                    return View();
                }

                // Find user by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed for email: {Email} - User not found", email);
                    ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                    return View();
                }

                // Check if user is locked out
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Login attempt failed for user {UserId} - Account locked out", user.Id);
                    ModelState.AddModelError("", "تم قفل الحساب مؤقتاً. يرجى المحاولة لاحقاً");
                    return View();
                }

                // Attempt to sign in
                var result = await _signInManager.PasswordSignInAsync(
                    user, 
                    password, 
                    isPersistent: rememberMe, 
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserId} logged in successfully", user.Id);
                    
                    // Redirect to return URL or home page
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {UserId} account locked out", user.Id);
                    ModelState.AddModelError("", "تم قفل الحساب مؤقتاً. يرجى المحاولة لاحقاً");
                }
                else if (result.RequiresTwoFactor)
                {
                    // Handle two-factor authentication if needed
                    ModelState.AddModelError("", "يتطلب هذا الحساب مصادقة ثنائية");
                }
                else
                {
                    _logger.LogWarning("Login attempt failed for user {UserId} - Invalid password", user.Id);
                    ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login attempt for email: {Email}", email);
                ModelState.AddModelError("", "حدث خطأ أثناء تسجيل الدخول. يرجى المحاولة مرة أخرى");
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await _signInManager.SignOutAsync();
                
                if (!string.IsNullOrEmpty(userId))
                {
                    _logger.LogInformation("User {UserId} logged out successfully", userId);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult AccessDenied()
        {
            TempData["AccessDeniedMessage"] = "عذراً، ليس لديك صلاحية للوصول لهذه الصفحة";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // Generate password reset token
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        
                        // Create reset link
                        var resetLink = Url.Action("ResetPassword", "Account", 
                            new { email = model.Email, code = token }, 
                            Request.Scheme, Request.Host.Value);

                        // Send email
                        var emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>();
                        var emailSent = await emailService.SendPasswordResetEmailAsync(model.Email, resetLink, user.UserName ?? model.Email);

                        if (emailSent)
                        {
                            _logger.LogInformation("Password reset email sent to {Email}", model.Email);
                            TempData["SuccessMessage"] = "تم إرسال رابط استعادة كلمة المرور إلى بريدك الإلكتروني. يرجى التحقق من صندوق الوارد.";
                        }
                        else
                        {
                            _logger.LogWarning("Failed to send password reset email to {Email}", model.Email);
                            TempData["ErrorMessage"] = "حدث خطأ أثناء إرسال البريد الإلكتروني. يرجى المحاولة مرة أخرى.";
                        }
                    }
                    else
                    {
                        // Don't reveal that the user does not exist
                        TempData["SuccessMessage"] = "إذا كان هذا البريد الإلكتروني مسجل في نظامنا، سيتم إرسال رابط استعادة كلمة المرور إليه.";
                    }

                    return RedirectToAction(nameof(ForgotPassword));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during forgot password process for email: {Email}", model.Email);
                    TempData["ErrorMessage"] = "حدث خطأ أثناء معالجة طلبك. يرجى المحاولة مرة أخرى.";
                    return RedirectToAction(nameof(ForgotPassword));
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                TempData["ErrorMessage"] = "رابط استعادة كلمة المرور غير صحيح.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "المستخدم غير موجود.";
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Code = code
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                            _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
                            TempData["SuccessMessage"] = "تم تغيير كلمة المرور بنجاح. يمكنك الآن تسجيل الدخول بكلمة المرور الجديدة.";
                            return RedirectToAction(nameof(Login));
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "المستخدم غير موجود.";
                        return RedirectToAction(nameof(Login));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during password reset for email: {Email}", model.Email);
                    TempData["ErrorMessage"] = "حدث خطأ أثناء إعادة تعيين كلمة المرور. يرجى المحاولة مرة أخرى.";
                    return RedirectToAction(nameof(Login));
                }
            }

            return View(model);
        }
    }
} 