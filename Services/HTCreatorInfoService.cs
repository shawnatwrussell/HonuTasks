using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services
{
    public class HTCreatorInfoService : IHTCreatorInfoService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<HTUser> _userManager;

        public HTCreatorInfoService(ApplicationDbContext context,
            UserManager<HTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Get a List of all the Users in a Company
        public async Task<List<HTUser>> GetAllMembersAsync(int creatorId)
        {
            List<HTUser> htUsers = new();

            htUsers = await _context.Users.Where(u => u.CreatorId == creatorId).ToListAsync();

            return htUsers;
        }

        //Get a List of all the Projects in a Company
        public async Task<List<Events>> GetAllEventsAsync(int creatorId)
        {
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

        //Get a List of all the Tasks by a Creator
        public async Task<List<Tasks>> GetAllTasksAsync(int creatorId)
        {
            List<Tasks> tasks = new();

            List<Events> events = new();

            //Get ALL the Tickets Connected to the ProjectId, which is connected to the CompanyId
            events = await GetAllEventsAsync(creatorId);

            tasks = events.SelectMany(p => p.Tasks).ToList();

            return tasks;
        }

        //Get Specified info related to a Specific Company
        public async Task<Creator> GetCreatorInfoByIdAsync(int? creatorId)
        {
            Creator creator = new();

            if (creatorId != null)
            {
                creator = await _context.Creator
                                         .Include(c => c.Members)
                                         .Include(c => c.Events)
                                         .Include(c => c.Invites)
                                         .FirstOrDefaultAsync(c => c.Id == creatorId);
            }

            return creator;
        }

        //Get a List of Members Assigned to a Specific Role
        public async Task<List<HTUser>> GetMembersInRoleAsync(string roleName, int creatorId)
        {
            List<HTUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();

            List<HTUser> roleUsers = users.Where(u => u.CreatorId == creatorId).ToList();

            return roleUsers;
        }

        public Task<Creator> GetCompanyInfoByIdAsync(int? creatorId)
        {
            throw new NotImplementedException();
        }
    }
}
