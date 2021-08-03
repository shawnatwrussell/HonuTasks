using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class Event
    {
        public int Id { get; set; }

        public int? EventPriorityId { get; set; }

        public int? EventStatusId { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Event Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Started")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Archived")]
        public DateTimeOffset? ArchivedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Created")]
        public DateTimeOffset Created { get; set; }


        //Image File Properties
        [NotMapped]
        [DataType(DataType.Upload)]
        //[MaxFileSize(1024 * 1024)]
        //[AllowedExtensions(new string[] {".jpg", ",png"})]
        [DisplayName("Choose a File")]
        public IFormFile ImageFormFile { get; set; }

        [DisplayName("File Name")]
        public string ImageFileName { get; set; }
        public byte[] ImageFileData { get; set; }

        [DisplayName("File Extension")]
        public string ImageFileContentType { get; set; }


        //Navigational Properties

        public virtual ICollection<HTUser> Members { get; set; } = new HashSet<HTUser>();

        public virtual EventPriority EventPriority { get; set; }
        public virtual EventStatus EventStatus { get; set; }

        public virtual ICollection<Task> Tasks { get; set; } = new HashSet<Task>();

    }
}
