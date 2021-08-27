using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models.ViewModels
{
    public class AssignUserViewModel
    {
        public SelectList AssignedUsers { get; set; }

        public string AssignedUserId { get; set; }

        public Tasks Tasks { get; set; }
    }
}
