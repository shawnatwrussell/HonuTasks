using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTHistoryService
    {
        Task AddHistoryAsync(Tasks oldTicket, Tasks newTask, string userId);

        Task<List<TaskHistory>> GetProjectTicketHistoriesAsync(int eventId);

        Task<List<TaskHistory>> GetCompanyTicketHistoriesAsync(int creatorId);

    }
}
