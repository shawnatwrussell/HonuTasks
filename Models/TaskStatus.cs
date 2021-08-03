using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class TaskStatus
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Task Status")]
        public string Name { get; set; }

    }
}
