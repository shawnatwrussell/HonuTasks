using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        #region Tasks Settings
        public IEnumerable<TaskPriority> Priorities { get; set; }

        public IEnumerable<TasksStatus> Statuses { get; set; }

        public IEnumerable<TaskType> Types { get; set; }
        #endregion

        #region Event Creation Wizard
        public IEnumerable<HTUser> DevUsers { get; set; }

        public IEnumerable<HTUser> SubUsers { get; set; }

        public IEnumerable<HTUser> EventPM { get; set; }
        #endregion

        #region User Roles and Management
        public IEnumerable<HTUser> AllUsers { get; set; }

        public IEnumerable<string> Roles { get; set; }
        #endregion    

    }
}
