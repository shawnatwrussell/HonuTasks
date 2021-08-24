using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Models.ViewModels;
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
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<HTUser> _userManager;
        private readonly IHTCreatorInfoService _infoService;
        private readonly IHTTaskService _ticketService;
        private readonly IHTEventService _projectService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<HTUser> userManager,
            IHTCreatorInfoService infoService,
            IHTTaskService ticketService,
            IHTEventService projectService
            )
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _infoService = infoService;
            _ticketService = ticketService;
            _projectService = projectService;
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
                Events = await _projectService.GetAllProjectsByCompany(creatorId),
                Tasks = await _ticketService.GetAllTicketsByCompanyAsync(creatorId),
                Members = await _infoService.GetAllMembersAsync(creatorId),
                DevTickets = await _ticketService.GetAllTicketsByRoleAsync("Developer", user.Id),
                SubmittedTickets = await _ticketService.GetAllTicketsByRoleAsync("Submitter", user.Id),
                CurrentUser = user,

                //UnassignedTickets = (await _ticketService.GetAllTicketsByTypeAsync(companyId, "Unassigned")).ToList(),

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

            List<Tasks> tasks = (await _taskService.GetAllTasksByCompanyAsync(creatorId)).OrderBy(t => t.Id).ToList();
            List<TasksStatus> statuses = _context.TasksStatus.ToList();
            ChartViewModel chartData = new();
            chartData.labels = tasks.Select(t => t.TaskStatus.Name).Distinct().ToArray();

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
            int companyId = User.Identity.GetCreatorId().Value;
            Random rnd = new();

            List<Tasks> tasks = (await _taskService.GetAllTasksByCreatorAsync(creatorId)).OrderBy(t => t.Id).ToList();
            List<TaskPriority> priorities = _context.TicketPriority.ToList();
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
            int companyId = User.Identity.GetCreatorId().Value;
            Random rnd = new();

            List<Task> tickets = (await _taskService.GetAllTicketsByCompanyAsync(creatorId)).OrderBy(t => t.Id).ToList();
            List<TaskType> types = _context.TaskType.ToList();
            ChartViewModel chartData = new();
            chartData.labels = tickets.Select(p => p.TaskType.Name).Distinct().ToArray();

            List<SubData> dsArray = new();
            List<int> numberOfTickets = new();
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
    }
