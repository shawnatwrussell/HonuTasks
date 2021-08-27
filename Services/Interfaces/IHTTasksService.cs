using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTTasksService
    {
        Task AssignTasksAsync(int taskId, string userId);

        Task<HTUser> GetTasksDeveloperAsync(int taskId);

        Task<List<Tasks>> GetAllTasksByCreatorAsync(int creatorId);

        Task<List<Tasks>> GetArchivedTasksByCreatorAsync(int creatorId);

        Task<List<Tasks>> GetAllTasksByPriorityAsync(int creatorId, string priorityName);

        Task<List<Tasks>> GetAllTasksByStatusAsync(int creatorId, string statusName);

        Task<List<Tasks>> GetAllTasksByTypeAsync(int creatorId, string typeName);

        Task<List<Tasks>> GetAllPMTasksAsync(string userId);

        Task<List<Tasks>> GetAllTasksByRoleAsync(string role, string userId);

        Task<List<Tasks>> GetEventTasksByRoleAsync(string role, string userId, int eventId);

        Task<List<Tasks>> GetEventTasksByStatusAsync(int eventId, int creatorId, string statusName);

        Task<List<Tasks>> GetEventTasksByPriorityAsync(int eventId, int creatorId, string statusName);

        Task<List<Tasks>> GetEventTasksByTypeAsync(int eventId, int creatorId, string statusName);

        Task<List<Tasks>> GetMyTasks(string role, string userId);

        //Task<int> IsThisMyTicket(int id);

        Task<int?> LookupTasksPriorityIdAsync(string priorityName);

        Task<int?> LookupTasksStatusIdAsync(string statusName);

        Task<int?> LookupTasksTypeIdAsync(string typeName);

    }
}
