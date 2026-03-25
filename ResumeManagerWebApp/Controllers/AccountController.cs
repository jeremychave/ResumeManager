using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApp.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction(nameof(DocumentsController.Index), "Documents");
        }

        var redirectUrl = Url.Action(nameof(DocumentsController.Index), "Documents", null, Request.Scheme);

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> Logout()
    {
        var redirectUrl = Url.Action(nameof(DocumentsController.Index), "Home", null, Request.Scheme);

        return SignOut(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }
}
