using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class TaskType
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Task Type")]
        public string Name { get; set; }

    }
}
