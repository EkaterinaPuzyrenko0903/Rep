using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payments.Data;
using Payments.Models;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ExcelDataReader;
using System.Text;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Generators;
using BCrypt;

namespace Payments.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger, ApplicationContext applicationContext)
        {
            _logger = logger;
            _context = applicationContext;
        }

        [HttpGet]
        public IActionResult Index()
        {


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            // Проверяем, что оба поля заполнены
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                // Если какое-то поле не заполнено, возвращаем сообщение об ошибке
                ModelState.AddModelError(string.Empty, "Пожалуйста, заполните все поля.");
                return View();
            }

            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    var claims = new List<Claim> {
                        new Claim("Id", user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.Name),
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Home");
                }
            }

            // Если пользователь не найден или пароль неверен, добавляем сообщение об ошибке
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
            return View();
        }

        [HttpGet]
        public IActionResult CreateDefaultUser()
        {
            var user = new User
            {
                Email = "m",
                Password = BCrypt.Net.BCrypt.HashPassword("1")
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }
        
    }
}