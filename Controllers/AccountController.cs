using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignalR8.Models;
using SignalR8.ViewModels;

namespace SignalR8.Controllers;

public class AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : Controller
{
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            //login
            var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

            if (result.Succeeded)
            {
                // return RedirectToLocal(returnUrl);
                return RedirectToAction("Index","Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");
        }
        return View(model);
    }

    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            AppUser user = new()
            {
                Name = model.Name,
                UserName = model.Email,
                Email = model.Email,
                Address = model.Address
            };

            var result = await userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, false);

                // return RedirectToLocal(returnUrl);
                return RedirectToAction("Index","Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index","Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction(nameof(HomeController.Index), nameof(HomeController));
    }
}