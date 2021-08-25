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
    public class HTInviteService : IHTInviteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHTEventService _eventService;
        private readonly IEmailSender _emailService;

        public HTInviteService(ApplicationDbContext context, IHTEventService eventService, IEmailSender emailService)
        {
            _context = context;
            _eventService = eventService;
            _emailService = emailService;
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email)
        {
            Invite invite = await _context.Invite.Include(i => i.CreatorId)
                                                 .Include(i => i.Event)
                                                 .Include(i => i.Invitor)
                                                 .FirstOrDefaultAsync(i => i.CreatorToken == token && i.InviteeEmail == email);

            return invite;
        }

        public async Task<Invite> GetInviteAsync(int id)
        {
            Invite invite = await _context.Invite.Include(i => i.CreatorId)
                                     .Include(i => i.Event)
                                     .Include(i => i.Invitor)
                                     .FirstOrDefaultAsync(i => i.Id == id);

            return invite;

        }

        public async Task<bool> AnyInviteAsync(Guid token, string email)
        {
            return await _context.Invite.AnyAsync(i => i.CreatorToken == token && i.InviteeEmail == email && i.IsValid == true);
        }

        public async Task<bool> AcceptInviteAsync(Guid? code, string userId)
        {
            Invite invite = await _context.Invite.FirstOrDefaultAsync(i => i.CreatorToken == code);

            if (invite == null)
            {
                return false;
            }

            try
            {
                //invite.IsValid = false;
                invite.InviteeId = userId;
                await _context.SaveChangesAsync();


                return true;
            }

            catch
            {
                throw;
            }
        }

        public async Task<bool> ValidateInviteCodeAsync(Guid? code)
        {
            if (code == null)
            {
                return false;
            }

            var invite = await _context.Invite.FirstOrDefaultAsync(i => i.CreatorToken == code);

            if ((DateTime.Now - (await _context.Invite.FirstOrDefaultAsync(i => i.CreatorToken == code)).InviteDate).TotalDays <= 7)
            {
                bool result = (await _context.Invite.FirstOrDefaultAsync(i => i.CreatorToken == code)).IsValid;

                return result;
            }

            else
            {
                return false;
            }
        }
    }
}
