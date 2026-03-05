using System;
using Microsoft.AspNetCore.Mvc;

namespace mvc.Controllers
{
	public class PageController : Controller
	{
        public IActionResult Welcome()
        {
            return View();

        }

        // Задание 2

        [Route("Page/Greet/{name?}")]
        public IActionResult Greet(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "Гость";
            }
            ViewBag.Name = name;
            return View();
        }



        // Задание 3

        [HttpGet]
        public IActionResult Edit()
        {
            ViewBag.Message = "Пусто";
            return View();
        }

        [HttpPost]
        public IActionResult Edit(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Вы ничего не ввели";
            }

            ViewBag.Message = message;
            return View("EditResult");
        }

        
        // Задание 4
        
    }
}

