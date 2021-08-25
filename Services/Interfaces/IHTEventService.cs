using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTEventService
    {
        public Task<bool> IsUserOnEvent(string userId, int eventId);

        public Task<bool> AddUserToEventAsync(string userId, int eventId);

        public Task RemoveUsersFromEventAsync(string userId, int eventId);

        public Task RemoveUsersFromEventByRoleAsync(string userId, int eventId);

        public Task<List<Events>> ListUserEventsAsync(HTUser user);

        public Task<List<Events>> GetAllEventsByCreator(int creatorId);

        public Task<List<Events>> GetArchivedEventsByCreator(int creatorId);

        public Task<List<Events>> GetAllEventsByPriority(int creatorId, string priorityName);

        public Task<List<HTUser>> GetEventMembersByRoleAsync(int eventId, string role);

        public Task<HTUser> GetEventManagerAsync(int eventId);

        public Task<List<Events>> GetAllEventsByRoleAsync(string role, string userId);

        public Task<bool> AddEventManagerAsync(string userId, int eventId);

        public Task RemoveEventManagerAsync(int eventId);

        public Task<List<HTUser>> GetMembersWithoutPMAsync(int eventId);

        public Task<List<HTUser>> UsersNotOnEventAsync(int eventId, int creatorId);

        public Task<IEnumerable<HTUser>> DevelopersOnEvent(int eventId);

        public Task<IEnumerable<HTUser>> SubmittersOnEvent(int eventId);

    }
}
