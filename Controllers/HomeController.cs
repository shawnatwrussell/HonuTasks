using HonuTasks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyInfoService _infoService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTProjectService _projectService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<BTUser> userManager,
            IBTCompanyInfoService infoService,
            IBTTicketService ticketService,
            IBTProjectService projectService
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
            int companyId = User.Identity.GetCompanyId().Value;
            BTUser user = await _userManager.GetUserAsync(User);

            DashboardViewModel model = new()
            {
                Projects = await _projectService.GetAllProjectsByCompany(companyId),
                Tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId),
                Members = await _infoService.GetAllMembersAsync(companyId),
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
            int companyId = User.Identity.GetCompanyId().Value;
            Random rnd = new();

            List<Ticket> tickets = (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).OrderBy(t => t.Id).ToList();
            List<TicketStatus> statuses = _context.TicketStatus.ToList();
            ChartViewModel chartData = new();
            chartData.labels = tickets.Select(t => t.TicketStatus.Name).Distinct().ToArray();

            List<SubData> dsArray = new();
            List<int> numberOfTickets = new();
            List<string> colors = new();

            foreach (var status in chartData.labels.ToList())
            {
                var statusId = statuses.FirstOrDefault(s => s.Name == status).Id;

                numberOfTickets.Add(tickets.Where(t => t.TicketStatusId == statusId).Count());

                //This code will randomly select a color for each element of the data
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:x6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = numberOfTickets.ToArray(),
                backgroundColor = colors.ToArray()
            };

            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }

        public async Task<JsonResult> PieMethod()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Random rnd = new();

            List<Ticket> tickets = (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).OrderBy(t => t.Id).ToList();
            List<TicketPriority> priorities = _context.TicketPriority.ToList();
            ChartViewModel chartData = new();
            chartData.labels = tickets.Select(p => p.TicketPriority.Name).Distinct().ToArray();

            List<SubData> dsArray = new();
            List<int> numberOfTickets = new();
            List<string> colors = new();

            foreach (var priority in chartData.labels.ToList())
            {
                var priorityId = priorities.FirstOrDefault(p => p.Name == priority).Id;

                numberOfTickets.Add(tickets.Where(t => t.TicketPriorityId == priorityId).Count());

                //This code will randomly select a color for each element of the data
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:x6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = numberOfTickets.ToArray(),
                backgroundColor = colors.ToArray()
            };

            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }

        public async Task<JsonResult> PieTwoMethod()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Random rnd = new();

            List<Ticket> tickets = (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).OrderBy(t => t.Id).ToList();
            List<TicketType> types = _context.TicketType.ToList();
            ChartViewModel chartData = new();
            chartData.labels = tickets.Select(p => p.TicketType.Name).Distinct().ToArray();

            List<SubData> dsArray = new();
            List<int> numberOfTickets = new();
            List<string> colors = new();

            foreach (var type in chartData.labels.ToList())
            {
                var typeId = types.FirstOrDefault(p => p.Name == type).Id;

                numberOfTickets.Add(tickets.Where(t => t.TicketTypeId == typeId).Count());

                //This code will randomly select a color for each element of the data
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:x6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = numberOfTickets.ToArray(),
                backgroundColor = colors.ToArray()
            };

            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }
