using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int Id { get; set; }
        public int EventUserId { get; set; }
        public int EventId { get; set; }


        public List<Events> Events { get; set; }
        public Events Name { get; set; }
        public Tasks Title { get; set; }
        public List<Tasks> Tasks { get; set; }
        public List<Tasks> AssignedTasks { get; set; }
        public List<Tasks> CreatedTasks { get; set; }
        public List<HTUser> Members { get; set; }
        public HTUser CurrentUser { get; set; }
        public Array[] ChartData { get; set; }
        public List<TaskType> UnassignedTasks { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<TasksStatus> TaskStatuses { get; set; }
        public List<EventStatus> EventStatuses { get; set; }

    }
}
