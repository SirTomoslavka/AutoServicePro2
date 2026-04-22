using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceApp.Controllers;

[Authorize]
public class AccountController : Controller
{
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
