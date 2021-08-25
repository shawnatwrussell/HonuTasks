using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTHistoryService
    {
        Task AddHistoryAsync(Tasks oldTask, Tasks newTask, string userId);

        Task<List<TaskHistory>> GetEventTaskHistoriesAsync(int eventId);

        Task<List<TaskHistory>> GetCreatorTaskHistoriesAsync(int creatorId);

    }
}
