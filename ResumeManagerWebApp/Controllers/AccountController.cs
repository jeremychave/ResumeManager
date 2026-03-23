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
            return RedirectToAction(nameof(DocumentsController.Index), "Document");
        }

        var redirectUrl = Url.Action(nameof(DocumentsController.Index), "Document", null, Request.Scheme);

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    //public IActionResult Logout()
    //{
    //    return SignOut(
    //        new AuthenticationProperties { RedirectUri = "/" },
    //        CookieAuthenticationDefaults.AuthenticationScheme,
    //        OpenIdConnectDefaults.AuthenticationScheme
    //    );
    //}

    //public IActionResult AccessDenied()
    //{
    //    return View();
    //}
}
