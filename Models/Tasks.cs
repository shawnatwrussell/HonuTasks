using HonuTasks.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class Tasks
    {
        internal bool userId;

        //Primary Key
        public int Id { get; set; }

        //Foreign Key
        [DisplayName("Event")]
        public int EventId { get; set; }  //was ProjectId in BT

        [DisplayName("User")]
        public string UserId { get; set; }

        [DisplayName("Task Priority")]
        public int TaskPriorityId { get; set; }  //was TicketPriorityId in BT

        [DisplayName("Task Status")]
        public int TaskStatusId { get; set; }  //was TicketStatusId in BT

        [DisplayName("Task Type")]
        public int TaskTypeId { get; set; }  //was TicketTypeId in BT

        [DisplayName("Task Creator")]
        public string OwnerUserId { get; set; }

        [DisplayName("Assigned User")]
        public string AssignedUserId { get; set; }  //was DeveloperUserId in BT

        [Required]
        [StringLength(50)]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Created")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Updated")]
        public DateTimeOffset? Updated { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Archived")]
        public DateTimeOffset? ArchivedDate { get; set; }

        [Required] //selection on drop-down needed
        [Display(Name = "Task Status")]
        public TasksStatuses TasksStatuses { get; set; } //link to enum



        //Navigational Properties
        public virtual Events Event { get; set; }

        public virtual HTUser User { get; set; }
        public virtual HTUser OwnerUser { get; set; }
        public virtual HTUser AssignedUser { get; set; }

        public virtual TaskPriority TaskPriority { get; set; }

        public virtual TasksStatus TasksStatus { get; set; }

        public virtual TaskType TaskType { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; } =
            new HashSet<Notification>();

        public virtual ICollection<TaskAttachment> Attachments { get; set; } =
            new HashSet<TaskAttachment>();

        public virtual ICollection<TaskHistory> History { get; set; } =
            new HashSet<TaskHistory>();

        public virtual ICollection<TaskComment> Comments { get; set; } =
            new HashSet<TaskComment>();

        //"virtual" = grabs it from the Db when needed

        public virtual IEnumerable<Tasks> AssignedTasks { get; set; }
        public virtual IEnumerable<Tasks> CreatorTasks { get; set; }


    }
}
