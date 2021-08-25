using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services
{
    public class HTRolesService : IHTRolesService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<HTUser> _userManager;
        private readonly IHTCreatorInfoService _infoService;

        public HTRolesService(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<HTUser> userManager,
            IHTCreatorInfoService infoService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _infoService = infoService;
        }

        public async Task<bool> AddUserToRoleAsync(HTUser user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;

            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<string> GetRoleNameByIdAsync(HTUser user, string roleId)
        {
            IdentityRole role = _context.Roles.Find(roleId);

            return await _roleManager.GetRoleNameAsync(role);
        }

        public async Task<bool> IsUserInRoleAsync(HTUser user, string roleName)
        {
            bool result = await _userManager.IsInRoleAsync(user, roleName);

            return result;
        }

        public async Task<IEnumerable<string>> ListUserRolesAsync(HTUser user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user);

            return result;
        }

        public async Task<bool> RemoveUserFromRoleAsync(HTUser user, string roleName)
        {
            var result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;

            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<List<HTUser>> UsersNotInRoleAsync(string roleName, int creatorId)
        {
            List<HTUser> usersNotInRole = new();
            try
            {
                //TODO: Modify for multi-tenants
                foreach (HTUser user in await _infoService.GetAllMembersAsync(creatorId))
                {
                    if (!await IsUserInRoleAsync(user, roleName))
                    {
                        usersNotInRole.Add(user);
                    }
                }
            }

            catch
            {

                throw;
            }

            return usersNotInRole;
        }

        public async Task<bool> RemoveUserFromRolesAsync(HTUser users, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(users, roles)).Succeeded;

            await _context.SaveChangesAsync();
            return result;
        }
    }
}
