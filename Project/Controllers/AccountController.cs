using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationContext _applicationContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _applicationContext = applicationContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _applicationContext.Users.FirstOrDefaultAsync(p=> p.UserName == (model.Phone.ToString()));
                if (user == null)
                {
                    return View(model);
                }

                Microsoft.AspNetCore.Identity.SignInResult result =
                await _signInManager.PasswordSignInAsync(user,
                model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        IList<string> userRoles = await _userManager.GetRolesAsync(user);
                        return userRoles.FirstOrDefault(p => p == "admin") != null ?
                        RedirectToAction("index", "admin") : userRoles.FirstOrDefault(p => p == "curator") != null ?
                        RedirectToAction("index", "curator") : RedirectToAction("index", "runner");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Register(string id)
        {
            Link element = await _applicationContext.Links.FirstOrDefaultAsync(p => p.Url == id && p.Active == true);
            if (element != null)
                {
                    RegisterViewModel model = new RegisterViewModel()
                    {
                        UserId = element.UserId
                    };
                ViewData["Users"] = new SelectList(await _userManager.GetUsersInRoleAsync("curator"), "Id", "Name");
                return View(model);
                }
                else return NotFound();

        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Runner runner = new Runner {Phone = model.Phone, Surname = model.Surname, Name = model.Name, Patronymic = model.Patronymic, UserId = model.UserId, Comment = model.Comment };
                _applicationContext.Add(runner);
                await _applicationContext.SaveChangesAsync();
                List<Marathon> marathons = _applicationContext.Marathons.ToList();
                    if (marathons.Count == 0)
                    {
                        Marathon marathon = new Marathon()
                        {
                            Number = 1,
                            StartRegister = DateTime.Now,
                            StartFlow = null,
                            EndFlow = null,
                            EndTask = null,
                            EndRegister = null
                        };
                        _applicationContext.Add(marathon);
                        await _applicationContext.SaveChangesAsync();

                    }
                    return RedirectToAction("index", "home");
                }
                else
                {
                ViewData["Users"] = new SelectList(await _userManager.GetUsersInRoleAsync("curator"), "Id", "Name");
                return View(model);
                }
            }
        [Authorize(Roles = "admin,curator")]
        [HttpGet]
        public async Task<IActionResult> EditUser()
        {
            string userId = _userManager.GetUserId(User);
            User user = await _applicationContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser([Bind("Id,Phone,Surname,Name,Patronymic,PasswordHash")] User user)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Phone.ToString();
                User existItem = await _applicationContext.Set<User>().FindAsync(user.Id);
                _applicationContext.Entry(existItem).CurrentValues.SetValues(user);
                int result = await _applicationContext.SaveChangesAsync();
                return RedirectToAction("editUser");
            }
            else
                return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

    }
}
