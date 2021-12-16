using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    [Authorize(Roles = "curator")]
    public class CuratorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationContext _applicationContext;
        public CuratorController(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            string user = _userManager.GetUserId(User);
            ViewData["Link"] = String.Concat(_httpContextAccessor.HttpContext.Request.Host.Value, "/account/register/");
            return View(_applicationContext.Links.FromSqlRaw("Select * From Links Where UserId = {0}", user));
        }
        [HttpGet]
        public IActionResult Recordings()
        {
            return View(_applicationContext.Recordings.Include(p => p.Marathon).Include(p => p.Runner));
        }
        [HttpGet]
        public IActionResult Runners()
        {
            string user = _userManager.GetUserId(User);
            var users = _applicationContext.Runners.Where(p => p.UserId == user).Include(p=> p.User);
            return View(users);
        }
        [HttpGet]
        public IActionResult ExcelRunner()
        {
            string user = _userManager.GetUserId(User);
            var runners = _applicationContext.Runners.Where(p => p.UserId == user).Include(p => p.User);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLink(int? id,bool active)
        {
            Link link = await _applicationContext.Links.FirstOrDefaultAsync(p => p.Id == id);
            if (link != null)
            {
                link.Active = active;
                _applicationContext.Update(link);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("links");
            }
            else
                return View(link);
        }
        [HttpGet]
        public async Task<IActionResult> SendClient(int? id)
        {
            if (id == null)
                return NotFound();
            Runner runner = await _applicationContext.Runners.FindAsync(id);
            if (runner == null)
                return NotFound();
            else
                ViewData["Users"] = new SelectList(await _userManager.GetUsersInRoleAsync("curator"), "Id", "Name");
            return View(runner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendClient([Bind("Id,Phone,Surname,Name,Patronymic,Comment,UserId")] Runner runner)
        {
            if (ModelState.IsValid)
            {
                _applicationContext.Update(runner);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("users");
            }
            else
            ViewData["Users"] = new SelectList(await _userManager.GetUsersInRoleAsync("curator"), "Id", "Name");
            return View(runner);
        }
        [HttpGet]
        public IActionResult AddRecording()
        {
            ViewData["Link"] = new SelectList(_applicationContext.Links, "Id", "Url");
            ViewData["Marathon"] = new SelectList(_applicationContext.Marathons, "Id", "Number");
            ViewData["Runner"] = new SelectList(_applicationContext.Runners, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecording([Bind("Id,UserId,MarathonId,RunnerId")] Recording recording)
        {
            if (ModelState.IsValid)
            {
                recording.UserId = _userManager.GetUserId(User);
                _applicationContext.Add(recording);
                await _applicationContext.SaveChangesAsync();
                return RedirectToAction("recordings");
            }
            ViewData["Link"] = new SelectList(_applicationContext.Links, "Id", "Url");
            ViewData["Marathon"] = new SelectList(_applicationContext.Marathons, "Id", "Number");
            ViewData["Runner"] = new SelectList(_applicationContext.Runners, "Id", "Name");
            return View(recording);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
