using HonuTasks.Data;
using HonuTasks.Extensions;
using HonuTasks.Models;
using HonuTasks.Models.ViewModels;
using HonuTasks.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<HTUser> _userManager;
        private readonly IHTCreatorInfoService _infoService;
        private readonly IHTTasksService _tasksService;
        private readonly IHTEventService _eventService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<HTUser> userManager,
            IHTCreatorInfoService infoService,
            IHTTasksService tasksService,
            IHTEventService eventService
            )
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _infoService = infoService;
            _tasksService = tasksService;
            _eventService = eventService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Landing()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            int creatorId = User.Identity.GetCreatorId().Value;
            HTUser user = await _userManager.GetUserAsync(User);

            DashboardViewModel model = new()
            {
                Events = await _eventService.GetAllEventsByCreator(creatorId),
                Tasks = await _tasksService.GetAllTasksByCreatorAsync(creatorId),
                Members = await _infoService.GetAllMembersAsync(creatorId),
                AssignedTasks = await _tasksService.GetAllTasksByRoleAsync("AssignedUser", user.Id),
                CreatedTasks = await _tasksService.GetAllTasksByRoleAsync("Submitter", user.Id),
                CurrentUser = user,

                //UnassignedTasks = (await _tasksService.GetAllTasksByTypeAsync(creatorId, "Unassigned")).ToList(),

            };

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }



        //CHART DISPLAY
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<JsonResult> DonutMethod()
        {
            int creatorId = User.Identity.GetCreatorId().Value;
            Random rnd = new();

            List<Tasks> tasks = (await _tasksService.GetAllTasksByCreatorAsync(creatorId)).OrderBy(t => t.Id).ToList();
            List<TasksStatus> statuses = _context.TasksStatus.ToList();
            ChartViewModel chartData = new();
            chartData.labels = tasks.Select(t => t.TasksStatus.Name).Distinct().ToArray();

            List<SubData> dsArray = new();
            List<int> numberOfTasks = new();
            List<string> colors = new();

            foreach (var status in chartData.labels.ToList())
            {
                var statusId = statuses.FirstOrDefault(s => s.Name == status).Id;

                numberOfTasks.Add(tasks.Where(t => t.TaskStatusId == statusId).Count());

                //This code will randomly select a color for each element of the data
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:x6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = numberOfTasks.ToArray(),
                backgroundColor = colors.ToArray()
            };

            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }

        public async Task<JsonResult> PieMethod()
        {
            int creatorId = User.Identity.GetCreatorId().Value;
            Random rnd = new();

            List<Tasks> tasks = (await _tasksService.GetAllTasksByCreatorAsync(creatorId)).OrderBy(t => t.Id).ToList();
            List<TaskPriority> priorities = _context.TaskPriority.ToList();
            ChartViewModel chartData = new();
            chartData.labels = tasks.Select(p => p.TaskPriority.Name).Distinct().ToArray();

            List<SubData> dsArray = new();
            List<int> numberOfTasks = new();
            List<string> colors = new();

            foreach (var priority in chartData.labels.ToList())
            {
                var priorityId = priorities.FirstOrDefault(p => p.Name == priority).Id;

                numberOfTasks.Add(tasks.Where(t => t.TaskPriorityId == priorityId).Count());

                //This code will randomly select a color for each element of the data
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:x6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = numberOfTasks.ToArray(),
                backgroundColor = colors.ToArray()
            };

            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }

        public async Task<JsonResult> PieTwoMethod()
        {
            int creatorId = User.Identity.GetCreatorId().Value;
            Random rnd = new();

            List<Tasks> tasks = (await _tasksService.GetAllTasksByCreatorAsync(creatorId)).OrderBy(t => t.Id).ToList();
            List<TaskType> types = _context.TaskType.ToList();
            ChartViewModel chartData = new();
            chartData.labels = tasks.Select(p => p.TaskType.Name).Distinct().ToArray();

            List<SubData> dsArray = new();
            List<int> numberOfTasks = new();
            List<string> colors = new();

            foreach (var type in chartData.labels.ToList())
            {
                var typeId = types.FirstOrDefault(p => p.Name == type).Id;

                numberOfTasks.Add(tasks.Where(t => t.TaskTypeId == typeId).Count());

                //This code will randomly select a color for each element of the data
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:x6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = numberOfTasks.ToArray(),
                backgroundColor = colors.ToArray()
            };

            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }

        ////CALENDAR
        //public Task<IActionResult> Calendar()
        //{
        //}
    }
}

