using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationContext _applicationContext;

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _applicationContext = applicationContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Links()
        {
            await _applicationContext.Database.ExecuteSqlRawAsync("Exec DeleteRows");
            ViewData["Link"] = String.Concat(_httpContextAccessor.HttpContext.Request.Host.Value, "/account/register/");
            return View(_applicationContext.Links.FromSqlRaw("Select * From Links"));
        }
        [HttpGet]
        public IActionResult Marathons()
        {
            return View(_applicationContext.Marathons);
        }
        [HttpGet]
        public IActionResult Users()
        {
            return View(_applicationContext.Users);
        }
        [HttpGet]
        public async Task<IActionResult> Runners(string userId)
        {
            IQueryable<Runner> runners = _applicationContext.Runners.Include(x => x.User);
            if (userId != null)
            {
                runners = runners.Where(p => p.UserId == userId);
            }
            IList<User> users = await _userManager.GetUsersInRoleAsync("curator");
            users.Insert(0, new User { Name = "Все", Id = null });
            RunnersViewModel viewModel = new RunnersViewModel
            {
                Runners = runners.ToList(),
                Users = new SelectList(users, "Id", "Name")

            };
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Reviews()
        {
            return View(_applicationContext.Reviews);
        }
        [HttpGet]
        public IActionResult ExcelUser()
        {
            var users = _applicationContext.Users;
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Пользователи");
                int currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Телефон";
                worksheet.Cell(currentRow, 2).Value = "Фамилия";
                worksheet.Cell(currentRow, 3).Value = "Имя";
                worksheet.Cell(currentRow, 4).Value = "Отчество";
                worksheet.Cell(currentRow, 5).Value = "Роль";
                foreach (User user in users)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = user.Phone;
                    worksheet.Cell(currentRow, 2).Value = user.Surname;
                    worksheet.Cell(currentRow, 3).Value = user.Name;
                    worksheet.Cell(currentRow, 4).Value = user.Patronymic;
                    worksheet.Cell(currentRow, 5).Value = _applicationContext.Roles.FirstOrDefault(p => p.Id ==
                        _applicationContext.UserRoles.FirstOrDefault(p => p.UserId == user.Id).RoleId);
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "users.xlsx");
                }
            }
        }
        [HttpGet]
        public IActionResult ExcelRunner()
        {
            var runners = _applicationContext.Runners.Include(p => p.User);
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Клиенты");
                int currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Телефон";
                worksheet.Cell(currentRow, 2).Value = "Фамилия";
                worksheet.Cell(currentRow, 3).Value = "Имя";
                worksheet.Cell(currentRow, 4).Value = "Отчество";
                worksheet.Cell(currentRow, 5).Value = "Комментария";
                worksheet.Cell(currentRow, 6).Value = "Куратор";
                foreach (Runner runner in runners)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = runner.Phone;
                    worksheet.Cell(currentRow, 2).Value = runner.Surname;
                    worksheet.Cell(currentRow, 3).Value = runner.Name;
                    worksheet.Cell(currentRow, 4).Value = runner.Patronymic;
                    worksheet.Cell(currentRow, 5).Value = runner.Comment;
                    worksheet.Cell(currentRow, 6).Value = runner.User.Name;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "runners.xlsx");
                }
            }
        }
        [HttpGet]
        public IActionResult AddUser()
        {
            AddViewModel model = new AddViewModel
            {
                AllRoles = _roleManager.Roles.ToList()
        };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(AddViewModel model, string roles)
        {
            if (ModelState.IsValid)
            {
                User user = new User {UserName = model.Phone.ToString(), Phone = model.Phone, Surname = model.Surname, Name = model.Name, Patronymic = model.Patronymic};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {              
                    await _userManager.AddToRoleAsync(user, roles);
                    return RedirectToAction("index", "admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            model = new AddViewModel
            {
                AllRoles = _roleManager.Roles.ToList()
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AddLink()
        {
            ViewData["Users"] = new SelectList(await _userManager.GetUsersInRoleAsync("curator"), "Id", "Name");
            ViewData["Link"] = String.Concat(_httpContextAccessor.HttpContext.Request.Host.Value, "/account/register/");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLink([Bind("Id,Url,Active,ShelfLife,UserId")] Link link)
        {
            if (ModelState.IsValid)
            {
                _applicationContext.Add(link);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("links");
            }
            ViewData["Users"] = new SelectList(await _userManager.GetUsersInRoleAsync("curator"), "Id", "Name");
            return View(link);
        }
        [HttpGet]
        public IActionResult AddMarathon()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMarathon([Bind("Id,Number,StartRegister,EndRegister,StartFlow,EndFlow,EndTask")] Marathon marathon)
        {
            if (ModelState.IsValid)
            {
                marathon.Number = _applicationContext.Marathons.Count() + 1;
                marathon.StartRegister = DateTime.Now;
                marathon.EndRegister = null;
                marathon.EndFlow = null;
                marathon.EndTask = null;
                _applicationContext.Add(marathon);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("marathons");
            }
            return View(marathon);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            User user = await _applicationContext.Users.FindAsync(id);
            _applicationContext.Users.Remove(user);
            await _applicationContext.SaveChangesAsync();
            return RedirectToAction("users");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
