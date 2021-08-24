using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTCreatorInfoService
    {
        Task<Creator> GetCompanyInfoByIdAsync(int? creatorId);

        Task<List<HTUser>> GetAllMembersAsync(int creatorId);

        Task<List<Events>> GetAllEventsAsync(int creatorId);

        Task<List<Tasks>> GetAllTasksAsync(int creatorId);

        Task<List<HTUser>> GetMembersInRoleAsync(string roleName, int creatorId);

    }
}
