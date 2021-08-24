using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTRolesService
    {
        public Task<bool> IsUserInRoleAsync(HTUser user, string roleName);

        public Task<IEnumerable<string>> ListUserRolesAsync(HTUser user);
        //a Method that returns/gives a collection/list of Enumerated strings;
        //has to have a user of Type BTUser in order to work

        public Task<bool> AddUserToRoleAsync(HTUser user, string roleName);

        public Task<bool> RemoveUserFromRoleAsync(HTUser user, string roleName);

        public Task<bool> RemoveUserFromRolesAsync(HTUser user, IEnumerable<string> roles);

        public Task<List<HTUser>> UsersNotInRoleAsync(string roleName, int creatorId);

        public Task<string> GetRoleNameByIdAsync(HTUser user, string roleId);

        //public string ListUserRoles(BTUser user);

    }
}
