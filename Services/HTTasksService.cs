using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Models.Enums;
using HonuTasks.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services
{
    public class HTTasksService : IHTTasksService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<HTUser> _userManager;
        private readonly IHTCreatorInfoService _infoService;
        private readonly IHTRolesService _roleService;
        private readonly IHTEventService _eventService;
        private readonly IHttpContextAccessor _accessor;


        public HTTasksService(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<HTUser> userManager,
            IHTCreatorInfoService infoService,
            IHTRolesService roleService,
            IHTEventService eventService,
            IHttpContextAccessor accessor)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _infoService = infoService;
            _roleService = roleService;
            _eventService = eventService;
            _accessor = accessor;
        }

        public async Task AssignTasksAsync(int taskId, string userId)
        {
            //ADD User to a Specific Ticket
            //userId
            //taskId
            Tasks tasks = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

            if (tasks != null)
            {
                try
                {
                    tasks.TaskStatusId = (await LookupTasksStatusIdAsync("Development")).Value; //using a method that returns a nullable
                    tasks.AssignedUserId = userId;
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public async Task<List<Tasks>> GetAllPMTasksAsync(string userId)  //Removed: (int eventId)
        {
            //LIST All the Event Managers ASSIGNED to (all) Tasks
            var user = await _context.Users.FindAsync(userId);
            List<Events> events = await _eventService.ListUserEventsAsync(user);
            List<Tasks> tasks = events.SelectMany(t => t.Tasks).ToList();

            return tasks;
        }

        public async Task<List<Tasks>> GetAllTasksByCreatorAsync(int creatorId)
        {
            //LIST All the Tasks Owned by a Specific Creator via the specific Event
            try
            {
                List<Tasks> tasks = await _context.Events.Include(p => p.CreatorId)
                                                      .Where(p => p.CreatorId == creatorId)
                                                      .SelectMany(p => p.Tasks)
                                                             .Include(t => t.Attachments)
                                                             .Include(t => t.Comments)
                                                             .Include(t => t.History)
                                                             .Include(t => t.AssignedUser)
                                                             .Include(t => t.OwnerUser)
                                                             .Include(t => t.TaskPriority)
                                                             .Include(t => t.TasksStatus)
                                                             .Include(t => t.TaskType)
                                                             .Include(t => t.Event)
                                                             .ToListAsync();

                return tasks;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Tasks>> GetAllTasksByPriorityAsync(int creatorId, string priorityName)
        {
            //LIST All Tasks Given a Specific Priority(Id)
            int priorityId = (await LookupTasksPriorityIdAsync(priorityName)).Value;

            return await _context.Events.Where(p => p.CreatorId == creatorId)
                                                      .SelectMany(p => p.Tasks)
                                                             .Include(t => t.Attachments)
                                                             .Include(t => t.Comments)
                                                             .Include(t => t.History)
                                                             .Include(t => t.AssignedUser)
                                                             .Include(t => t.OwnerUser)
                                                             .Include(t => t.TaskPriority)
                                                             .Include(t => t.TasksStatus)
                                                             .Include(t => t.TaskType)
                                                             .Include(t => t.Event)
                                                             .Where(t => t.TaskPriorityId == priorityId)
                                                             .ToListAsync();
        }

        public async Task<List<Tasks>> GetAllTasksByRoleAsync(string role, string userId)
        {
            //LIST All Tasks with a Specific Role(roleName)
            List<Tasks> tasks = new();

            if (string.Compare(role, Roles.AssignedUser.ToString()) == 0)
            {
                try
                {
                    tasks = await _context.Tasks
                                            .Include(t => t.Attachments)
                                            .Include(t => t.Comments)
                                            .Include(t => t.AssignedUser)
                                            .Include(t => t.OwnerUser)
                                            .Include(t => t.TaskPriority)
                                            .Include(t => t.TasksStatus)
                                            .Include(t => t.TaskType)
                                            .Include(t => t.Event)
                                                .ThenInclude(p => p.Members)
                                            .Include(t => t.Event)
                                                .ThenInclude(p => p.EventPriority)
                                            .Where(t => t.AssignedUserId == userId).ToListAsync();

                }
                catch
                {
                    throw;
                }
            }
            else if (string.Compare(role, Roles.OwnerUser.ToString()) == 0)
            {
                try
                {
                    tasks = await _context.Tasks
                                            .Include(t => t.Attachments)
                                            .Include(t => t.Comments)
                                            .Include(t => t.AssignedUser)
                                            .Include(t => t.OwnerUser)
                                            .Include(t => t.TaskPriority)
                                            .Include(t => t.TasksStatus)
                                            .Include(t => t.TaskType)
                                            .Include(t => t.Event)
                                                .ThenInclude(p => p.Members)
                                            .Include(t => t.Event)
                                                .ThenInclude(p => p.EventPriority)
                                            .Where(t => t.OwnerUserId == userId)
                                            .ToListAsync();
                }
                catch
                {
                    throw;
                }

            }
            else if (string.Compare(role, Roles.EventManager.ToString()) == 0)
            {
                tasks = await GetAllPMTasksAsync(userId);
            }

            return tasks;

        }

        public async Task<List<Tasks>> GetEventTasksByStatusAsync(int eventId, int creatorId, string statusName)
        {
            //LIST All Tasks with a Specific Status(Id)
            int statusId = (await LookupTasksStatusIdAsync(statusName)).Value;
            List<Tasks> tasks = new();

            try
            {
                tasks = await _context.Events.Where(p => p.CreatorId == creatorId)
                                                      .SelectMany(p => p.Tasks)
                                                             .Include(t => t.Attachments)
                                                             .Include(t => t.Comments)
                                                             .Include(t => t.AssignedUser)
                                                             .Include(t => t.OwnerUser)
                                                             .Include(t => t.TaskPriority)
                                                             .Include(t => t.TasksStatus)
                                                             .Include(t => t.TaskType)
                                                             .Include(t => t.Event)
                                                             .Where(t => t.TaskStatusId == statusId)
                                                             .ToListAsync();
            }
            catch
            {
                throw;
            }
            return tasks;
        }

        public async Task<List<Tasks>> GetEventTasksByPriorityAsync(int eventId, int creatorId, string priorityName)
        {
            //LIST All Tasks with a Specific Status(Id)
            int priorityId = (await LookupTasksPriorityIdAsync(priorityName)).Value;
            List<Tasks> tasks = new();

            try
            {
                tasks = await _context.Events.Where(p => p.CreatorId == creatorId)
                                                      .SelectMany(p => p.Tasks)
                                                             .Include(t => t.Attachments)
                                                             .Include(t => t.Comments)
                                                             .Include(t => t.AssignedUser)
                                                             .Include(t => t.OwnerUser)
                                                             .Include(t => t.TaskPriority)
                                                             .Include(t => t.TasksStatus)
                                                             .Include(t => t.TaskType)
                                                             .Include(t => t.Event)
                                                             .Where(t => t.TaskPriorityId == priorityId)
                                                             .ToListAsync();
            }
            catch
            {
                throw;
            }
            return tasks;
        }

        public async Task<List<Tasks>> GetEventTasksByTypeAsync(int eventId, int creatorId, string typeName)
        {
            //LIST All Tasks with a Specific Status(Id)
            int typeId = (await LookupTasksTypeIdAsync(typeName)).Value;
            List<Tasks> tasks = new();

            try
            {
                tasks = await _context.Events.Where(p => p.CreatorId == creatorId)
                                                      .SelectMany(p => p.Tasks)
                                                             .Include(t => t.Attachments)
                                                             .Include(t => t.Comments)
                                                             .Include(t => t.AssignedUser)
                                                             .Include(t => t.OwnerUser)
                                                             .Include(t => t.TaskPriority)
                                                             .Include(t => t.TasksStatus)
                                                             .Include(t => t.TaskType)
                                                             .Include(t => t.Event)
                                                             .Where(t => t.TaskTypeId == typeId)
                                                             .ToListAsync();
            }
            catch
            {
                throw;
            }
            return tasks;
        }

        public async Task<List<Tasks>> GetAllTasksByStatusAsync(int creatorId, string statusName)
        {
            //LIST All Tasks with a Specific Status(Id)
            int statusId = (await LookupTasksStatusIdAsync(statusName)).Value;

            return await _context.Events.Where(p => p.CreatorId == creatorId)
                                                      .SelectMany(p => p.Tasks)
                                                             .Include(t => t.Attachments)
                                                             .Include(t => t.Comments)
                                                             .Include(t => t.History)
                                                             .Include(t => t.AssignedUser)
                                                             .Include(t => t.OwnerUser)
                                                             .Include(t => t.TaskPriority)
                                                             .Include(t => t.TasksStatus)
                                                             .Include(t => t.TaskType)
                                                             .Include(t => t.Event)
                                                             .Where(t => t.TaskStatusId == statusId)
                                                             .ToListAsync();

        }

        public async Task<List<Tasks>> GetAllTasksByTypeAsync(int creatorId, string typeName)
        {
            //LIST All Tickets with a Specific Type(Id)
            int typeId = (await LookupTasksTypeIdAsync(typeName)).Value;

            return await _context.Events.Where(p => p.CreatorId == creatorId)
                                                      .SelectMany(p => p.Tasks)
                                                             .Include(t => t.Attachments)
                                                             .Include(t => t.Comments)
                                                             .Include(t => t.History)
                                                             .Include(t => t.AssignedUser)
                                                             .Include(t => t.OwnerUser)
                                                             .Include(t => t.TaskPriority)
                                                             .Include(t => t.TasksStatus)
                                                             .Include(t => t.TaskType)
                                                             .Include(t => t.Event)
                                                             .Where(t => t.TaskTypeId == typeId)
                                                             .ToListAsync();
        }

        public async Task<List<Tasks>> GetArchivedTasksByCreatorAsync(int creatorId)
        {
            //LIST All the Tasks ARCHIVED with a Specific CreatorId
            try
            {
                List<Tasks> tasks = await _context.Events.Where(p => p.CreatorId == creatorId)
                                                      .SelectMany(p => p.Tasks)
                                                             .Include(t => t.Attachments)
                                                             .Include(t => t.Comments)
                                                             .Include(t => t.History)
                                                             .Include(t => t.AssignedUser)
                                                             .Include(t => t.OwnerUser)
                                                             .Include(t => t.TaskPriority)
                                                             .Include(t => t.TasksStatus)
                                                             .Include(t => t.TaskType)
                                                             .Include(t => t.Event)
                                                             .Where(t => t.Archived == true)
                                                             .ToListAsync();

                return tasks;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Tasks>> GetEventTasksByRoleAsync(string role, string userId, int eventId)
        {
            //LIST All tasks with a Specific EventId AND a Specific roleName
            List<Tasks> tasks = (await GetAllTasksByRoleAsync(role, userId)).Where(t => t.EventId == eventId).ToList();

            return tasks;
        }

        public async Task<List<Tasks>> GetMyTasks(string role, string userId)
        {
            List<Tasks> myTasks = (await GetAllTasksByRoleAsync(role, userId)).ToList();

            return myTasks;
        }

        public async Task<HTUser> GetTasksDeveloperAsync(int taskId)
        {
            //LIST All Assigned User with a Specific TaskId

            HTUser developer = new();
            Tasks tasks = await _context.Tasks.Include(t => t.AssignedUser).FirstOrDefaultAsync(t => t.Id == taskId);

            if (tasks?.AssignedUserId != null)  //? means: is there a task?
            {
                developer = tasks.AssignedUser;
            }
            return developer;
        }

        public async Task<int?> LookupTasksPriorityIdAsync(string priorityName) //returns a Nullable value
        {
            try
            {
                TaskPriority priority = await _context.TaskPriority.FirstOrDefaultAsync(p => p.Name == priorityName);
                return priority?.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int?> LookupTasksStatusIdAsync(string statusName)  //returns a Nullable value
        {
            try
            {
                TasksStatus status = await _context.TasksStatus.FirstOrDefaultAsync(p => p.Name == statusName);
                return status?.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int?> LookupTasksTypeIdAsync(string typeName)  //returns a Nullable value
        {
            try
            {
                TaskType type = await _context.TaskType.FirstOrDefaultAsync(p => p.Name == typeName);
                return type?.Id;
            }
            catch
            {
                throw;
            }
        }

        //public Task<int> IsThisMyTicket(int id)
        // {
        // var ticket = _context.Ticket.Find(id);
        // var user = _accessor.HttpContext.User;
        //  var btUser = _context.Users.Find(_userManager.GetUserId(user));

        // if (user.IsInRole(Roles.Admin.ToString())) return true;
        //     else if (user.IsInRole(Roles.Developer.ToString())) return ticket.DeveloperUserId == btUser.Id;
        //     else if (user.IsInRole(Roles.Submitter.ToString())) return ticket.OwnerUserId == btUser.Id;
        //     else if (user.IsInRole(Roles.ProjectManager.ToString()))
        //         {
        //             var myTicketIds = btUser.Projects.SelectMany(p => p.Tickets).Select(t => t.Id);
        //            return myTicketIds.Contains(id);
        //}

        //else return false;
        //}
    }
}