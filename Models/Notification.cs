using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [DisplayName("Task")]
        public int TaskId { get; set; }

        [Required]
        [DisplayName("Subject")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string Message { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string RecipientId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string SenderId { get; set; }

        [DisplayName("Has Been Viewed")]
        public bool Viewed { get; set; }


        //Navigational Properties
        public virtual Tasks Task { get; set; }

        public virtual HTUser Recipient { get; set; }

        public virtual HTUser Sender { get; set; }

    }
}
