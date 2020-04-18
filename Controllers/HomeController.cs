using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RegLog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace RegLog.Controllers
{
    
    public class HomeController : Controller
    {
        private MyContext _context {get;set;}
        public HomeController(MyContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("signon")]
        public IActionResult Singon()
        {
            return View("LogIn");
        }
        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Members.Any (m => m.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    _context.Members.Add(newUser);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("userinSession", newUser.UserId);
                    return Redirect($"{newUser.UserId}");
                }
            }
            else
            {
                return View("Index");
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser userIn)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _context.Members.FirstOrDefault (m => m.Email == userIn.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email Address");
                    return View("Index");
                }
                else
                {
                    var hash = new PasswordHasher<LoginUser>();
                    var result = hash.VerifyHashedPassword(userIn, userInDb.Password, userIn.LoginPassword);
                    if (result == 0)
                    {
                        ModelState.AddModelError("LoginPassword", "Invalid Password");
                        return View("Index");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32 ("userinSession", userInDb.UserId);
                        Console.WriteLine("***********************");
                        Console.WriteLine(userInDb.UserId);
                        return Redirect($"{userInDb.UserId}");
                    }
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("{LoggedinID}")]
        public IActionResult Success(int LoggedinID)
        {
            int? userid = HttpContext.Session.GetInt32 ("userinSession");
            if(userid == LoggedinID)
            {
                User Loggedin = _context.Members.FirstOrDefault(m => m.UserId == LoggedinID);
                return View("Success",Loggedin);
            }
            else
            {
                return View("Index");
            }
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userinSession");
            HttpContext.Session.Clear();
            return Redirect("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
