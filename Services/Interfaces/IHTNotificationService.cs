using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTNotificationService
    {
        public Task SaveNotificationAsync(Notification notification);

        public Task AdminsNotificationAsync(Notification notification, int creatorId);

        public Task MembersNotificationAsync(Notification notification, List<HTUser> members);

        public Task EmailNotificationAsync(Notification notification, string emailSubject);

        public Task SMSNotificationAsync(string phone, Notification notification);

        public Task<List<Notification>> GetReceivedNotificationsAsync(string userId);

        public Task<List<Notification>> GetSentNotificationsAsync(string userId);

    }
}
