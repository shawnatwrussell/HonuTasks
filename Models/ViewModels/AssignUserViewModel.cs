using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models.ViewModels
{
    public class AssignUserViewModel
    {
        public SelectList Users { get; set; }

        public string UserId { get; set; }

        public Tasks Task { get; set; }
    }
}
