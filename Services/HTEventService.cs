using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Models.Enums;
using HonuTasks.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services
{
    public class HTEventService : IHTEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<HTUser> _userManager;
        private readonly IHTCreatorInfoService _infoService;
        private readonly IHTRolesService _roleService;

        public HTEventService(ApplicationDbContext context,
               RoleManager<IdentityRole> roleManager,
               UserManager<HTUser> userManager,
               IHTCreatorInfoService infoService,
               IHTRolesService roleService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _infoService = infoService;
            _roleService = roleService;
        }

        public async Task<bool> AddEventManagerAsync(string userId, int eventId)
        {
            //Need to ASSIGN a Event Manager User to a Specific Event
            //Only ONE Event Manager/event=check for existing Event Manager
            //IS a User ASSIGNED the ROLE of Event Manager? yes/no
            Events events = await _context.Events.FindAsync(eventId);
            HTUser user = await _userManager.FindByIdAsync(userId);
            events.Members.Add(user);

            bool result = await _userManager.IsInRoleAsync(user, "Event Manager");

            await _context.SaveChangesAsync();  //Updates the Database
            return await AddEventManagerAsync(userId, eventId);
        }

        public async Task<bool> AddUserToEventAsync(string userId, int eventId)
        {
            //Need to Assign a User to a Specific Event
            Events events = await _context.Events.FindAsync(eventId);
            HTUser user = await _userManager.FindByIdAsync(userId);
            events.Members.Add(user);

            bool result = await _userManager.IsInRoleAsync(user, "User");

            await _context.SaveChangesAsync();  //Updates the Database
            return result;

        }

        public async Task<List<Events>> GetAllEventsByCreator(int creatorId)
        {
            //List ALL the Events Owned by a Specific CreatorId
            //Grab all the Events with Same CreatorId from the Database
            //Return a List of the Events

            //USe HTCreatorInfoService: GetAllEventsAsync

            List<Events> events = new();

            events = await _context.Events.Include(p => p.Members)
                                             .Include(p => p.EventPriority)
                                             .Include(p => p.Tasks)
                                                .ThenInclude(t => t.OwnerUser)
                                             .Include(p => p.Tasks)
                                                .ThenInclude(t => t.AssignedUser)
                                             .Include(p => p.Tasks)
                                                .ThenInclude(t => t.Comments)
                                             .Include(p => p.Tasks)
                                                .ThenInclude(t => t.Attachments)
                                             .Include(p => p.Tasks)
                                                .ThenInclude(t => t.History)
                                             .Include(p => p.Tasks)
                                                .ThenInclude(t => t.TaskPriority)
                                             .Include(p => p.Tasks)
                                                .ThenInclude(t => t.TaskStatus)
                                             .Include(p => p.Tasks)
                                                .ThenInclude(t => t.TaskType)
                                             .Where(p => p.CreatorId == creatorId).ToListAsync();

            return events;
        }

        public async Task<List<Events>> GetAllEventsByPriority(int creatorId, string priorityName)
        {
            //List ALL the Events Given a Specific Priority
            //Need creatorId
            //Need priorityId
            //List<Events> events = await GetAllEventsByCreator(creatorId);
            //List<Events> eventPriority = await _context.Events.Include(p => p.EventPriority).ToListAsync();

            //return eventPriority;
            List<Events> events = await GetAllEventsByCreator(creatorId);

            EventPriority eventPriority = await _context.EventPriority.FirstOrDefaultAsync(p => p.Name == priorityName);

            return events.Where(p => p.EventPriorityId == eventPriority.Id).ToList();

        }

        public async Task<List<Events>> GetArchivedEventsByCreator(int creatorId)
        {
            //List ALL the Archived Events Owned by a Specific Creator
            //Use GetAllEventsByCreator
            //narrow down to Archived-status of the events
            List<Events> events = new();

            events = await GetAllEventsByCreator(creatorId);

            return events.Where(p => p.Archived == true).ToList();
        }

        public async Task<List<HTUser>> GetMembersWithoutPMAsync(int eventId)
        {
            //List All Users NOT Assigned in the a Event Manager ROLE
            List<HTUser> developers = await GetEventMembersByRoleAsync(eventId, "Assigned User");
            List<HTUser> submitters = await GetEventMembersByRoleAsync(eventId, "Submitter");
            List<HTUser> admins = await GetEventMembersByRoleAsync(eventId, "Admin");

            List<HTUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }

        public async Task<HTUser> GetEventManagerAsync(int eventId)
        {
            //LIST of Event Managers
            Events events = await _context.Events
                                    .Include(events => events.Members)
                                    .FirstOrDefaultAsync(userId => userId.Id == eventId);

            foreach (HTUser member in events?.Members)
            {
                if (await _roleService.IsUserInRoleAsync(member, "Event Manager"))
                {
                    return member;
                }
            }
            return null;
        }

        public async Task<List<HTUser>> GetEventMembersByRoleAsync(int eventId, string role)
        {
            //LIST of Developers Assigned to the Specific EventId

            //Grab all Event Members
            Events events = _context.Events
                                    .Include(p => p.Members)
                                    .FirstOrDefault(u => u.Id == eventId);

            List<HTUser> members = new();

            foreach (var user in events.Members)
            {
                //Is the Member a Developer?
                if (await _roleService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }
            return members;

        }

        public async Task<IEnumerable<HTUser>> DevelopersOnEvent(int eventId)
        {
            var developers = await _userManager.GetUsersInRoleAsync("Assigned User");
            var onProject = await GetMembersWithoutPMAsync(eventId);
            var devsOnProject = new List<HTUser>();
            devsOnProject.AddRange(developers.Intersect(onProject).ToList());

            return devsOnProject;
        }

        public async Task<IEnumerable<HTUser>> SubmittersOnEvent(int eventId)
        {
            var submitters = await _userManager.GetUsersInRoleAsync("Submitter");
            var onProject = await GetMembersWithoutPMAsync(eventId);
            var subsOnProject = new List<HTUser>();
            subsOnProject.AddRange(submitters.Intersect(onProject).ToList());

            return subsOnProject;
        }

        public async Task<bool> IsUserOnEvent(string userId, int eventId)
        {
            //Is a Specific User Assigned to a Specific Event? yes/no
            if (userId != null)
            {
                var devs = await DevelopersOnEvent(eventId);
                var subs = await SubmittersOnEvent(eventId);
            }

            return false;
        }

        public async Task<List<HTUser>> UsersNotOnEventAsync(int eventId, int creatorId)
        {
            //LIST All the Users Not Currently Assigned to Any Events
            List<HTUser> usersNotOnEvent = new();
            usersNotOnEvent = await _context.Users.Where(u => u.Events.All(p => p.Id != eventId) && u.CreatorId == creatorId).ToListAsync();

            return usersNotOnEvent.ToList();
        }

        public async Task<List<Events>> ListUserEventsAsync(HTUser user)
        {

            List<Events> result = await _context.Events
                                                .Where(p => p.Members.Contains(user))
                                                .Include(p => p.EventPriority)
                                                .Include(p => p.EventStatus)
                                                .Include(p => p.CreatorId)
                                                .Include(p => p.Members)
                                                .Include(p => p.Tasks).ToListAsync();

            return result;
        }

        public async Task RemoveEventManagerAsync(int eventId)
        {
            //UNASSIGN a Specific Event Manager from a Specific Event
            foreach (var user in (await GetEventMembersByRoleAsync(eventId, "Event Manager")))
            {
                await RemoveUsersFromEventAsync(user.Id, eventId);
            }
        }

        public async Task RemoveUsersFromEventAsync(string userId, int eventId)
        {
            //UNASSIGN a Specific User from a Specific Event
            Events events = await _context.Events.FindAsync(eventId);
            HTUser user = await _userManager.FindByIdAsync(userId);
            events.Members.Remove(user);

            await _context.SaveChangesAsync();

        }

        public async Task RemoveUsersFromEventByRoleAsync(string userId, int eventId)
        {
            //UNASSIGN Users from a Event Based on Specific Role
            Events events = await _context.Events.FindAsync(eventId);
            HTUser users = await _userManager.FindByIdAsync(userId);
            events.Members.Remove(users);

            await _context.SaveChangesAsync();

        }

        public async Task<List<Events>> GetAllEventsByRoleAsync(string role, string userId)
        {
            //LIST All Events assigned to a Specific User [Role(roleName)]
            List<Events> events = new();

            if (string.Compare(role, Roles.AssignedUser.ToString()) == 0)
            {
                try
                {
                    events = await _context.Events
                                            .Include(p => p.EventPriority)
                                            .Include(p => p.EventStatus)
                                            .Include(p => p.CreatorId)
                                            .Include(p => p.Members)
                                            .Include(p => p.Tasks).ToListAsync();

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
                    events = await _context.Events
                                            .Include(p => p.EventPriority)
                                            .Include(p => p.EventStatus)
                                            .Include(p => p.CreatorId)
                                            .Include(p => p.Members)
                                            .Include(p => p.Tasks).ToListAsync();
                }
                catch
                {
                    throw;
                }

            }
            else if (string.Compare(role, Roles.EventManager.ToString()) == 0)
            {
                var user = await _context.Users.FindAsync(userId);
                events = await ListUserEventsAsync(user);
            }

            return events;

        }
    }
}
