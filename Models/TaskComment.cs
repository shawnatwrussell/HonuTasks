using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class TaskComment
    {
        //Primary Key
        public int Id { get; set; }

        //Foreign Key
        [DisplayName("Task")]
        public int TaskId { get; set; }

        [DisplayName("User")]
        public string UserId { get; set; }

        [DisplayName("Task Comment")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Member Comment")]
        public string Comment { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created Date")]
        public DateTimeOffset Created { get; set; }


        //Navigational Properties
        public virtual Task Task { get; set; }
        //References the Parent-property: Task class

        public virtual HTUser User { get; set; }
        //References the User, making the comment

    }
}
