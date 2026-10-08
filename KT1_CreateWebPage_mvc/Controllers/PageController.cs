using Microsoft.AspNetCore.Mvc;
namespace KT1_CreateWebPage_mvc.Controllers;

public class PageController : Controller
{
    // Задание 1
    public IActionResult Welcome()
    {
        return View();
    }
    
    // Задание 2
    [Route("Page/Greet/{name}")]
    public IActionResult Greet(string name)
    {
        ViewBag.Name = name;
        return View();
    }
    
    // Задание 3
    public IActionResult Edit()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Edit(string message)
    {
        ViewBag.Message = message;
        return View("Edit",  message);
    }
}