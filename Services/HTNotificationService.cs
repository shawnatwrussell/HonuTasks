using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services
{
    public class HTNotificationService : IHTNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailService;
        private readonly IHTCreatorInfoService _infoService;

        public HTNotificationService(ApplicationDbContext context,
            IEmailSender emailService,
            IHTCreatorInfoService infoService)
        {
            _context = context;
            _emailService = emailService;
            _infoService = infoService;
        }


        //send a notification to a specific company's Admin, to let them know a ticket was created
        public async Task AdminsNotificationAsync(Notification notification, int creatorId)
        {
            try
            {
                //Get company admin
                List<HTUser> admins = await _infoService.GetMembersInRoleAsync("Admin", creatorId);

                foreach (HTUser htUser in admins)
                {
                    notification.RecipientId = htUser.Id;

                    await EmailNotificationAsync(notification, notification.Title);
                }
            }

            catch
            {
                throw;
            }
        }

        public async Task EmailNotificationAsync(Notification notification, string emailSubject)
        {
            HTUser htUser = await _context.Users.FindAsync(notification.RecipientId);

            //Send Email
            string htUserEmail = htUser.Email;
            string message = notification.Message;

            try
            {
                await _emailService.SendEmailAsync(htUserEmail, emailSubject, message);
            }

            catch
            {
                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            List<Notification> notifications = await _context.Notification
                                                                     .Include(n => n.Recipient)
                                                                     .Include(n => n.Sender)
                                                                     .Include(n => n.Task)
                                                                         .ThenInclude(t => t.Event)
                                                                     .Where(n => n.RecipientId == userId).ToListAsync();
            return notifications;
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            List<Notification> notifications = await _context.Notification
                                                         .Include(n => n.Recipient)
                                                         .Include(n => n.Sender)
                                                         .Include(n => n.Task)
                                                             .ThenInclude(t => t.Event)
                                                         .Where(n => n.SenderId == userId).ToListAsync();
            return notifications;

        }

        public Task MembersNotificationAsync(Notification notification, List<HTUser> members)
        {
            throw new NotImplementedException();
        }

        public async Task SaveNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }

            catch
            {
                throw;
            }
        }

        public Task SMSNotificationAsync(string phone, Notification notification)
        {
            throw new NotImplementedException();
        }
    }
}
