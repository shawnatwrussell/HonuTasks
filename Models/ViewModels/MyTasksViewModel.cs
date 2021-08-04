using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models.ViewModels
{
    public class MyTasksViewModel
    {
        public IEnumerable<Tasks> AssignedTasks { get; set; }

        public IEnumerable<Tasks> CreatedTasks { get; set; }

    }
}
