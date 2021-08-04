
using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    interface IHTUserInfoService
    {
        Task<EventUser> GetUserInfoByIdAsync(int? eventUserId);

        Task<List<HTUser>> GetAllMembersAsync(int eventUserId);

        Task<List<Event>> GetAllProjectsAsync(int userId);

        Task<List<Tasks>> GetAllTicketsAsync(int eventUserId);

        Task<List<HTUser>> GetMembersInRoleAsync(string roleName, int eventUserId);

    }
}
