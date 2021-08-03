using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class EventStatus
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Event Status")]
        public string Name { get; set; }

    }
}
