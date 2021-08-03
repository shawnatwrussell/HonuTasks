using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class EventPriority
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Event Priority")]
        public string Name { get; set; }

    }
}
