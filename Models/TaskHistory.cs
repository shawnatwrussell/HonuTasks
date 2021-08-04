using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class TaskHistory  //ReadOnly; User can NOT modify
    {
        //Primary Key
        public int Id { get; set; }

        //Foreign Key
        [DisplayName("Task")]
        public int TaskId { get; set; }

        [DisplayName("User")]
        public string UserId { get; set; }

        [DisplayName("Description of Changes")]
        public string Description { get; set; }

        [DisplayName("Updated Item")]
        public string Property { get; set; }

        [DisplayName("Previous")]
        public string OldValue { get; set; }

        [DisplayName("Current")]
        public string NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }


        //Navigational Properties
        public virtual Tasks Task { get; set; }
        //References the Parent-property: Task class

        public virtual HTUser User { get; set; }

    }
}
