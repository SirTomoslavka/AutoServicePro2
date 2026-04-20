using AutoServiceApp.Data;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
